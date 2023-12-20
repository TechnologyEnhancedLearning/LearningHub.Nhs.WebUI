var sco = {
	InstanceId: "",
	Version: "",
	Children: "",
	StudentId: "",
	StudentName: "",
	LessonLocation: "",
	Credit: "",
	LessonStatus: "ab-initio",
	Exit: "",
	Entry: "",
	Score_Children: "",
	Score_Raw: "",
	MasteryScore: "",
	SessionTime: "",
	TotalTime: "",
	SuspendData: "",
	LaunchData: "",
	Interactions_Children: "",
	Interactions_Count: "",
	Interactions: {},
	Objectives_Children: "",
	Objectives_Count: "",
	Objectives: {},
	Objectives_Score_Children: ""
};

function logLMSError(errorStr) {

	alert("Error communicating with the server.  Your learning details may not have been saved.\n\n" +
		"If the error persists please email support@elfh.org.uk for further assistance.");

}

var API = {
	"name": "e-lfh LMS API",
	LMSLastErrorCode: 0,
	LMSLastErrorMsg: "",
	LMSGetLastError: function () { // returns the last error recorded by the LMS
		return this.LMSLastErrorCode;
	},
	LMSGetErrorString: function (errorCode) { // Provides a textual description of the input error code

		var result = "";
		if (errorCode == "0") {
			result = "No error"
		}
		else if (errorCode == "101") {
			result = "General exception"
		}
		else if (errorCode == "201") {
			result = "Invalid argument error"
		}
		else if (errorCode == "202") {
			result = "Element cannot have children"
		}
		else if (errorCode == "203") {
			result = "Element not an array - cannot have count"
		}
		else if (errorCode == "301") {
			result = "Not initialized"
		}
		else if (errorCode == "401") {
			result = "Not implemented error"
		}
		else if (errorCode == "402") {
			result = "Invalid set value, elementis a keyword"
		}
		else if (errorCode == "403") {
			result = "Element is read only"
		}
		else if (errorCode == "404") {
			result = "Element is write only"
		}
		else if (errorCode == "405") {
			result = "Incorrect Data Type"
		}

		//TODO: needs vendor specific + catchall adding
		return result;
	},
	LMSGetDiagnostic: function (errorCode) { // The vendor specific textual description that corresponds to the input error code
		//TODO: add more specific msgs for elfh codes
		return this.LMSLastErrorMsg;
	},
	LMSInitialize: function (str) { // initialise connection to the LMS, e.g. record the session start time

		API.LMSLastErrorCode = 0;
		API.LMSLastErrorMsg = "";

		var result = "false";
		var origURL = "";

		$.ajax({
			type: 'POST',
			url: '/api/Scorm/LMSInitialise/' + app.launch.componentHierarchyId,
			beforeSend: function (jqXHR, settings) {
				origURL = settings.url;
			},
			success: function (returnedSCO) {

				if (returnedSCO == "") {
					LogError("Unable to initialize LMS content for " + str, origURL);
				}
				sco = returnedSCO; // $.parseJSON(returnedSCO);
				result = "true";
			},
			error: function (jqHXR, textStatus, errorThrown) {
				API.LMSLastErrorCode = 301;
				//API.LMSLastErrorMsg = "Unable to connect to e-LfH LMS (" + jqHXR.status + " " + jqHXR.statusText + ")";
				API.LMSLastErrorMsg = "Unable to connect to e-LfH LMS";

				logLMSError(API.LMSGetLastError());
				LogError(jqHXR.statusText + '\n\n' + jqHXR.responseText, origURL);
				//reviewWin.close();
				result = "false";
			},
			async: false
		});

		return result;

	},
	LMSFinish: function (str) { // finish the communication with the LMS

		API.LMSLastErrorCode = 0;
		API.LMSLastErrorMsg = "";

		var result = "false";
		var scoJSON = JSON.stringify(sco);
		var origURL = "";
		var useAjax = true;

		if (navigator.sendBeacon !== undefined) {
			var form_data = new FormData();
			form_data.append('sco', scoJSON);
			if (navigator.sendBeacon('/api/Scorm/LMSFinish/', form_data)) {
				useAjax = false;
				result = "true";
			}
		}

		if (useAjax) {
			$.ajax({
				type: 'POST',
				url: '/api/Scorm/LMSFinish/',
				data: { sco: scoJSON },
				beforeSend: function (jqXHR, settings) {
					origURL = settings.url;
				},
				success: function (sResult) {
					result = "true";
				},
				error: function (event, jqHXR, textStatus, errorThrown) {

					var json = $.parseJSON(event.responseText);
					var errorMessage = json.errorMessage;

					if (json.StatusCode && json.StatusCode != 401 && json.StatusCode != 403) {

						API.LMSLastErrorCode = 301;
						API.LMSLastErrorMsg = "Unable to disconnect cleanly from the e-LfH LMS";
						logLMSError(API.LMSGetLastError());
						LogError(jqHXR.statusText + '\n\n' + errorMessage, origURL);
					}

					result = "false";
				},
				async: false
			});
		}

		window.opener.refreshDetails(app.launch.componentId);
		return result;
	},
	LMSGetValue: function (paramName) {

		API.LMSLastErrorCode = 0;
		API.LMSLastErrorMsg = "";

		var result = "";

		if (paramName === "cmi._version") {
			result = sco.version;
		}
		else if (paramName === "cmi.core._children") {
			result = sco.children;
		}
		else if (paramName === "cmi.core.student_id") {
			result = sco.studentId;
		}
		else if (paramName === "cmi.core.student_name") {
			result = sco.studentName;
		}
		else if (paramName === "cmi.core.lesson_location") {
			result = sco.lessonLocation;
		}
		else if (paramName === "cmi.core.credit") {
			result = sco.credit;
		}
		else if (paramName === "cmi.core.lesson_status") {
			result = sco.lessonStatus;
		}
		else if (paramName === "cmi.core.entry") {
			result = sco.entry;
		}
		else if (paramName === "cmi.core.score._children") {
			result = sco.score_Children;
		}
		else if (paramName === "cmi.core.score.raw") {
			result = sco.score_Raw;
		}
		else if (paramName === "cmi.core.total_time") {
			result = SecondsToTime(TimeToSeconds(sco.totalTime) + TimeToSeconds(sco.sessionTime));
		}
		else if (paramName === "cmi.suspend_data") {
			result = sco.suspendData;
		}
		else if (paramName === "cmi.launch_data") {
			result = sco.launchData;
		}
		else if (paramName === "cmi.objectives._count") {
			result = 0;
		}
		else if (paramName === "cmi.student_data.mastery_score") {
			result = sco.masteryScore;
		}
		else if (paramName === "cmi.interactions._children") {
			result = sco.interactions_Children;
		}
		else if (paramName === "cmi.interactions._count") {
			result = sco.interactions_Count;
		}
		else if (paramName === "cmi.objectives._children") {
			result = sco.objectives_Children;
		}
		else if (paramName === "cmi.objectives._count") {
			result = sco.objectives_Count;
		}
		else if (paramName === "cmi.objectives.score._children") {
			result = sco.objectives_Score_Children;
		}
		else if (paramName.indexOf("cmi.objectives.") != -1) {
			// get sequence number and property type from the objectives string,
			// e.g. cmi.objectives.0.id
			var segments = paramName.split(".");

			var seqNumber = segments[2];
			var propertyType = segments[3];

			// if no objections exists error.
			if (sco.objectives == undefined || sco.objectives[seqNumber] == undefined) {
				API.LMSLastErrorCode = 301;
				API.LMSLastErrorMsg = "Not initialized";
				result = "";
			}
			else {
				// get the objective from the collection
				var objective = sco.objectives[seqNumber];

				switch (propertyType) {
					case "id":
						result = objective.Id;
						break;
					case "score":
						switch (segments[4]) {
							case "_children":
								result = sco.objectives_Score_Children;
								break;
							case "raw":
								result = objective.score_Raw;
								break;
							case "max":
								result = objective.score_Max;
								break;
							case "min":
								result = objective.score_Min;
								break;
						}
						break;
					case "status":
						result = objective.status;
						break;
				}
				sco.objectives[seqNumber] = objective;

				sco.objectives_Count = sco.objectives.length;
			}
		}
		else {
			API.LMSLastErrorCode = 401;
			API.LMSLastErrorMsg = "Not implemented error";
		}
		return result;
	},
	LMSSetValue: function (paramName, paramValue) {

		API.LMSLastErrorCode = 0;
		API.LMSLastErrorMsg = "";

		var result = "";

		// set the value in the sco

		if (paramName === "cmi.core.lesson_location") {
			sco.lessonLocation = paramValue;
		}
		else if (paramName === "cmi.core.lesson_status") {
			sco.lessonStatus = paramValue;
		}
		else if (paramName === "cmi.core.score.raw") {
			sco.score_Raw = paramValue;
		}
		else if (paramName === "cmi.core.exit") {
			sco.exit = paramValue;
		}
		else if (paramName === "cmi.core.session_time") {
			sco.sessionTime = paramValue;
		}
		else if (paramName === "cmi.suspend_data") {
			sco.suspendData = paramValue;
		}
		else if (paramName.indexOf("cmi.interactions.") != -1) {
			// get sequence number and property type from the interactions string,
			// e.g. cmi.interactions.0.id
			var segments = paramName.split(".");

			var seqNumber = segments[2];
			var propertyType = segments[3];

			// if no interaction exists in this position, add it.
			if (sco.interactions == undefined) {
				sco.interactions = [];
			}

			if (seqNumber > sco.interactions.length - 1) {
				sco.interactions.push({
					sequenceNumber: seqNumber,
					id: "",
					type: "",
					result: "",
					studentResponse: ""
				});
			}

			sco.interactions.sort(sortBySeqNum);

			// get the interaction from the collection
			var interaction = sco.interactions[seqNumber];

			switch (propertyType) {
				case "id":
					interaction.id = paramValue;
					break;
				case "type":
					interaction.type = paramValue;
					break;
				case "result":
					interaction.result = paramValue;
					break;
				case "weighting":
					interaction.weighting = paramValue;
					break;
				case "latency":
					interaction.latency = paramValue;
					break;
				case "student_response":  //"cmi.interactions.0.student_response"
					interaction.studentResponse = paramValue;
					break;
				//case "objectives":  //"cmi.interactions.0.objectives.0.id"
				//    /*
				//    var objSeqId = segments[4];
				//    var objProp = segments[5];

				//    if (objSeqId > interaction.Objectives.length - 1) {
				//        interaction.Objectives.push({ 
				//            Id: objSeqId,
				//            Type: "",
				//            Result: ""                                   
				//        });
				//    }

				//    interaction.Objectives.sort(sortById);

				//    // get the interaction from the collection
				//    var objective = interaction.Objectives[objSeqId];

				//    // update objective object

				//    // write it back to the collection
				//    interaction.Objectives[objSeqId] = objective;
				//    */

				//    break;
				//case "correct_responses":
				//    // each correct answer is passed individually.  We don't need these at the moment.
				//    // "cmi.interactions.0.correct_responses.0.pattern"
				//    // "cmi.interactions.0.correct_responses.1.pattern"
				//    break;

			}

			sco.interactions[seqNumber] = interaction;
			sco.interactions_Count = sco.interactions.length;

		}
		else if (paramName.indexOf("cmi.objectives.") != -1) {
			// get sequence number and property type from the objectives string,
			// e.g. cmi.objectives.0.id
			var segments = paramName.split(".");

			var seqNumber = segments[2];
			var propertyType = segments[3];

			// if no objections exists in this position, add it.
			if (sco.objectives == undefined) {
				sco.objectives = [];
			}

			if (seqNumber > sco.objectives.length - 1) {
				sco.objectives.push({
					sequenceNumber: seqNumber,
					id: "",
					score_Raw: "",
					score_Max: "",
					score_Min: "",
					status: ""
				});
			}

			sco.objectives.sort(sortBySeqNum);

			// get the objective from the collection
			var objective = sco.objectives[seqNumber];

			switch (propertyType) {
				case "id":
					objective.id = paramValue;
					break;
				case "score":
					switch (segments[4]) {
						case "_children":
							API.LMSLastErrorCode = 402;
							API.LMSLastErrorMsg = "Invalid set value, element is a keyword";
							break;
						case "raw":
							objective.score_Raw = paramValue;
							break;
						case "max":
							objective.score_Max = paramValue;
							break;
						case "min":
							objective.score_Min = paramValue;
							break;
					}
					break;
				case "status":
					objective.status = paramValue;
					break;
			}
			sco.objectives[seqNumber] = objective;

			sco.objectives_Count = sco.objectives.length;
		}
		else if (paramName === "cmi.interactions._children") {
			API.LMSLastErrorCode = 402;
			API.LMSLastErrorMsg = "Invalid set value, element is a keyword";
		}
		else if (paramName === "cmi.interactions._count") {
			API.LMSLastErrorCode = 402;
			API.LMSLastErrorMsg = "Invalid set value, element is a keyword";
		}
		else if (paramName === "cmi.student_data.mastery_score") {
			API.LMSLastErrorCode = 403;
			API.LMSLastErrorMsg = "Element is read only";
		}
		else {
			API.LMSLastErrorCode = 201;
			API.LMSLastErrorMsg = "Parameter '" + paramName + "' not supported!";
		}

		return "true";

	},
	LMSCommit: function (str) {

		API.LMSLastErrorCode = 0;
		API.LMSLastErrorMsg = "";

		var result = "false";
		var scoJSON = JSON.stringify(sco);
		var origURL = "";
		var useAjax = true;

		//if (navigator.sendBeacon !== undefined) {
		//	var form_data = new FormData();
		//	form_data.append('sco', scoJSON);
		//	if (navigator.sendBeacon('/api/Scorm/LMSCommit/', form_data)) {
		//		useAjax = false;
		//		result = "true";
		//	}
		//}

		if (useAjax) {
			$.ajax({
				type: "POST",
				url: "/api/Scorm/LMSCommit",
				data: { sco: scoJSON },
				dataType: 'json',
				contentType: "application/json; charset=utf-8",
				beforeSend: function (jqXHR, settings) {
					origURL = settings.url;
				},
				success: function (sResult) {
					if (!sResult) {
						LogError("Error throw at LMS Commit from the e-LfH LMS for content : '\n\n'", '/api/Scorm/LMSCommit');
					}
					result = "true";
				},
				error: function (event, jqHXR, textStatus, errorThrown) {
					var errorMessage = "Unknown error";
					if ('responseText' in event) {
						var json = $.parseJSON(event.responseText);
						errorMessage = json.errorMessage;
					}
					if (json.StatusCode && json.StatusCode != 401 && json.StatusCode != 403) {

						API.LMSLastErrorCode = 101;
						API.LMSLastErrorMsg = "LMSCommit failed.";
						logLMSError(API.LMSGetLastError());
						LogError(jqHXR.statusText + '\n\n' + errorMessage, origURL);
					}

					result = "false";
				},
				async: false
			});
		}
		return result;
	},
	LMSDisconnect: function () {

		API.LMSLastErrorCode = 0;
		API.LMSLastErrorMsg = "";

		var result = "false";
		var origURL = "";

		$.ajax({
			type: "POST",
			url: "/api/Scorm/LMSDisconnect",
			beforeSend: function (jqXHR, settings) {
				origURL = settings.url;
			},
			success: function (sResult) {
				result = "true";
			},
			error: function (event, jqHXR, textStatus, errorThrown) {

				var json = $.parseJSON(event.responseText);
				var errorMessage = json.errorMessage;

				if (json.StatusCode && json.StatusCode != 401 && json.StatusCode != 403) {

					API.LMSLastErrorCode = 101;
					API.LMSLastErrorMsg = "LMSDisconnect failed.";
					logLMSError(API.LMSGetLastError());
					LogError(jqHXR.statusText + '\n\n' + errorMessage, origURL);
				}

				result = "false";
			},
			async: false
		});

		return result;
	}
};

function SecondsToTime(totalSeconds) {

	var intHours = Math.floor(totalSeconds / (60 * 60));
	var intMinutes = Math.floor((totalSeconds - (intHours * 60 * 60)) / 60);
	var intSeconds = Math.floor((totalSeconds - (intHours * 60 * 60) - (intMinutes * 60)));

	var hours = intHours.toString();
	var minutes = ("00" + intMinutes.toString()).substring(intMinutes.toString().length);
	var seconds = ("00" + intSeconds.toString()).substring(intSeconds.toString().length);

	return hours + ':' + minutes + ':' + seconds;

}

function TimeToSeconds(sessionTime) {
	if (sessionTime == "") {
		return 0;
	} else {
		var pos1 = sessionTime.indexOf(':') + 1;
		var pos2 = sessionTime.indexOf(':', pos1 + 1) + 1;
		var pos3 = sessionTime.indexOf('.', pos2 + 1) + 1;

		if (pos3 == 0) {
			pos3 = sessionTime.length + 1;
		}

		var Hours = sessionTime.substring(0, pos1 - 1);
		var Minutes = sessionTime.substring(pos1, pos2 - 1);
		var Seconds = sessionTime.substring(pos2, pos3 - 1);
		var intHours = parseInt(Hours);
		var intMinutes = parseInt(Minutes);
		var intSeconds = parseInt(Seconds);

		var totalSeconds = (intHours * 60 * 60) + (intMinutes * 60) + intSeconds;

		return totalSeconds;
	}
}

function sortBySeqNum(a, b) {
	if (parseInt(a.SequenceNumber, 10) < parseInt(b.SequenceNumber, 10))
		return -1;
	if (parseInt(a.SequenceNumber, 10) > parseInt(b.SequenceNumber, 10))
		return 1;
	return 0;
}
