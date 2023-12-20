//import axios from 'axios';
import * as $ from 'jquery';
import { ScoModel } from "./scoModel";
import { ScormError } from "./scormError";
import { ScormLogItem } from './scormLogItem';
import { CMIVocabularyExit, CMIDataType, CMIVocabularyStatus, CMIVocabularyInteraction, CMIVocabularyResult, CMIVocabularyTimeLimitAction, CMIVocabularyEntry, CMIVocabularyCredit, CMIVocabularyMode } from './cmiVocabulary';

export class ScormApiModel {
	loggingEnabled = true;
	trace: ScormLogItem[] = [];

	name: string;
	resourceReferenceId = 0;
	LMSLastErrorCode = 0;
	LMSLastErrorMsg = "";

	sco: ScoModel;

	constructor(resourceReferenceId: number) {
		this.resourceReferenceId = resourceReferenceId;
		this.name = "Learning Hub LMS API";
		this.sco = new ScoModel();
		this.LMSLastErrorCode = 0;
		this.LMSLastErrorMsg = "";
    }

	LMSGetLastError() { // returns the last error recorded by the LMS
		return this.LMSLastErrorCode;
	}

	LMSGetErrorString(errorCode: string) { // Provides a textual description of the input error code
		let result = "";
		switch (errorCode) {
			case "0":
				result = "No error"
				break;
			case "101":
				result = "General exception"
				break;
			case "201":
				result = "Invalid argument error"
				break;
			case "202":
				result = "Element cannot have children"
				break;
			case "203":
				result = "Element not an array - cannot have count"
				break;
			case "301":
				result = "Not initialized"
				break;
			case "401":
				result = "Not implemented error"
				break;
			case "402":
				result = "Invalid set value, element is a keyword"
				break;
			case "403":
				result = "Element is read only"
				break;
			case "404":
				result = "Element is write only"
				break;
			case "405":
				result = "Incorrect Data Type"
				break;
			default:
				// Specific errors could be generated
				result = "Unknown exception"
				break;
		}
		return result;
	}

	LMSGetDiagnostic (errorCode: string) { // The vendor specific textual description that corresponds to the input error code
		//TODO: add more specific msgs for elfh codes
		return this.LMSLastErrorMsg;
	}

	LMSInitialize (str: string): string { // initialise connection to the LMS, e.g. record the session start time

		this.LMSLastErrorCode = 0;
		this.LMSLastErrorMsg = "";

		const apiUrl = '/api/Scorm/LMSInitialise/' + this.resourceReferenceId.toString();

		let objSco = this.sco;
		let result = false;
		$.ajax({
			type: 'POST',
			url: apiUrl,
			success: function (returnedSCO) {

				if (returnedSCO === null) {
					this.LogError("Unable to initialize LMS content for " + str, apiUrl);
				}
				Object.assign(objSco, returnedSCO);
				if (objSco.scoreRaw === null) { objSco.scoreRaw = ''; }
				if (objSco.scoreMin === null) { objSco.scoreMin = ''; }
				if (objSco.scoreMax === null) { objSco.scoreMax = ''; }
				result = true;
			},
			error: function (jqHXR, textStatus, errorThrown) {
				this.LMSLastErrorCode = 301;
				this.LMSLastErrorMsg = "Unable to connect to Learning Hub LMS (" + jqHXR.status + " " + jqHXR.statusText + ")";

				this.logLMSError(this.LMSGetLastError());
				this.LogError(jqHXR.statusText + '\n\n' + jqHXR.responseText, apiUrl);
				result = false;
			},
			async: false
		});

		this.logStatement('LMSInitialize("' + str + '")', result.toString());
		return result.toString();
	}

	LMSFinish (str: string): string { // finish the communication with the LMS

		this.LMSLastErrorCode = 0;
		this.LMSLastErrorMsg = "";

		let result = false;
		const scoJSON = JSON.stringify(this.sco);
		const formData = new FormData();
		formData.append('sco', scoJSON);
		let useAjax = true;

		if (navigator.sendBeacon !== undefined) {
			if (navigator.sendBeacon('/api/Scorm/LMSFinish/', formData)) {
				useAjax = false;
				result = true;
			}
		}

		const apiUrl = '/api/Scorm/LMSFinishObj';

		if (useAjax) {
			$.ajax({
				type: "POST",
				url: apiUrl,
				data: scoJSON,
				dataType: 'json',
				contentType: "application/json; charset=utf-8",
				success: function (sResult) {
					result = true;
				},
				error: function (jqHXR, textStatus, errorThrown) {
					var errorMessage = jqHXR.status + ': ' + jqHXR.statusText
					if (jqHXR.status && jqHXR.status != 401 && jqHXR.status != 403) {

						this.LMSLastErrorCode = 301;
						this.LMSLastErrorMsg = "Unable to disconnect cleanly from the e-LfH LMS";

						alert("Error communicating with the LMS server.  Your learning details may not have been saved.");
					}

					result = false;
				},
				async: false
			});
		}

		this.logStatement('LMSFinish("' + str + '")', result.toString());

		return result.toString(); // The specification specifies a string return value
	}

	LMSGetValue (paramName: string) {

		this.LMSLastErrorCode = 0;
		this.LMSLastErrorMsg = "";

		let result = "";

		if (paramName === "cmi._version") {
			result = this.sco.version;
		}
		else if (paramName === "cmi.core._children") {
			result = this.sco.children;
		}
		else if (paramName === "cmi.core.student_id") {
			result = this.sco.studentId;
		}
		else if (paramName === "cmi.core.student_name") {
			result = this.sco.studentName;
		}
		else if (paramName === "cmi.core.lesson_location") {
			result = this.sco.lessonLocation;
		}
		else if (paramName === "cmi.core.lesson_mode") {
			result = this.sco.lessonMode;
		}
		else if (paramName === "cmi.core.credit") {
			result = this.sco.credit;
		}
		else if (paramName === "cmi.core.lesson_status") {
			result = this.sco.lessonStatus;
		}
		else if (paramName === "cmi.core.entry") {
			result = this.sco.entry;
		}
		else if (paramName === "cmi.core.score._children") {
			result = this.sco.scoreChildren;
		}
		else if (paramName === "cmi.core.score.raw") {
			result = this.sco.scoreRaw;
		}
		else if (paramName === "cmi.core.score.min") {
			result = this.sco.scoreMin;
		}
		else if (paramName === "cmi.core.score.max") {
			result = this.sco.scoreMax;
		}
		else if (paramName === "cmi.core.total_time") {
			result = this.SecondsToTime(this.TimeToSeconds(this.sco.totalTime) + this.TimeToSeconds(this.sco.sessionTime));
		}
		else if (paramName === "cmi.suspend_data") {
			result = this.sco.suspendData;
		}
		else if (paramName === "cmi.launch_data") {
			result = this.sco.launchData;
		}
		else if (paramName === "cmi.comments") {
			result = this.sco.comments;
		}
		else if (paramName === "cmi.comments_from_lms") {
			result = this.sco.commentsFromLms;
		}
		else if (paramName === "cmi.student_data._children") {
			result = this.sco.studentDataChildren;
		}
		else if (paramName === "cmi.student_data.mastery_score") {
			result = this.sco.studentDataMasteryScore;
		}
		else if (paramName === "cmi.student_data.max_time_allowed") {
			result = this.sco.studentDataMaxTimeAllowed;
		}
		else if (paramName === "cmi.student_data.time_limit_action") {
			result = this.sco.studentDataTimeLimitAction;
		}
		else if (paramName === "cmi.student_preference._children") {
			result = this.sco.studentPreferenceChildren;
		}
		else if (paramName === "cmi.student_preference.audio") {
			result = this.sco.studentPreferenceAudio;
		}
		else if (paramName === "cmi.student_preference.language") {
			result = this.sco.studentPreferenceLanguage;
		}
		else if (paramName === "cmi.student_preference.speed") {
			result = this.sco.studentPreferenceSpeed;
		}
		else if (paramName === "cmi.student_preference.text") {
			result = this.sco.studentPreferenceText;
		}
		else if (paramName === "cmi.objectives._children") {
			result = this.sco.objectivesChildren;
		}
		else if (paramName === "cmi.objectives._count") {
			result = this.sco.objectives.length.toString();
		}
		else if (paramName === "cmi.objectives.score._children") {
			result = this.sco.objectivesScoreChildren;
		}
		else if (paramName.indexOf("cmi.objectives.") !== -1) {
			// get sequence number and property type from the objectives string,
			// e.g. cmi.objectives.0.id
			const segments = paramName.split(".");

			const seqNumber: number = +segments[2];
			const propertyType = segments[3];

			const objective = this.sco.objectives[seqNumber];
			if (objective === undefined) {
				this.LMSLastErrorCode = 301;
				this.LMSLastErrorMsg = "Not initialized";
				result = "";
			}
			else {
				switch (propertyType) {
					case "id":
						result = objective.id.toString();
						break;
					case "score":
						switch (segments[4]) {
							case "_children":
								result = this.sco.objectivesScoreChildren.toString();
								break;
							case "raw":
								result = objective.scoreRaw.toString();
								break;
							case "max":
								result = objective.scoreMax.toString();
								break;
							case "min":
								result = objective.scoreMin.toString();
								break;
						}
						break;
					case "status":
						result = objective.status;
						break;
				}
				this.sco.objectives[seqNumber] = objective;

				this.sco.objectivesCount = this.sco.objectives.length;
			}
		}
		else if (paramName === "cmi.interactions._children") {
			result = this.sco.interactionsChildren;
		}
		else if (paramName === "cmi.interactions._count") {
			result = this.sco.interactions.length.toString();
		}
		else if (paramName.indexOf("cmi.interactions.") !== -1) {
			const segments = paramName.split(".");
			const interactionSeqNumber: number = +segments[2];
			const propertyType = segments[3];

			const interaction = this.sco.interactions[interactionSeqNumber];

			if (interaction === undefined) {
				this.LMSLastErrorCode = 301;
				this.LMSLastErrorMsg = "Not initialized";
				result = "";
			} else {
				switch (propertyType) {
					case "id":
					case "time":
					case "type":
					case "weighting":
					case "student_response":
					case "result":
					case "latency":
						this.LMSLastErrorCode = 404;
						this.LMSLastErrorMsg = "Element is write only";
						break;
					case "objectives": {
						const objectiveSeqNumber = segments[4];
						const objectiveProperty = segments[5];
						if (objectiveSeqNumber === "_count") {
							result = interaction.objectives.length.toString();
						}
						else if (objectiveProperty === "id") {
							this.LMSLastErrorCode = 404;
							this.LMSLastErrorMsg = "Element is write only";
							break;
						}
						else {
							this.LMSLastErrorCode = 401;
							this.LMSLastErrorMsg = "Not implemented error";
						}
						break;
					}
					case "correct_responses": {
						const correctResponseSeqNumber = segments[4];
						const correctResponseProperty = segments[5];
						if (correctResponseSeqNumber === "_count") {
							result = interaction.correctResponses.length.toString();
						}
						else if (correctResponseProperty === "pattern") {
							this.LMSLastErrorCode = 404;
							this.LMSLastErrorMsg = "Element is write only";
							break;
						}
						else {
							this.LMSLastErrorCode = 401;
							this.LMSLastErrorMsg = "Not implemented error";
						}
						break;
					}
				}
            }

		}
		//else if (paramName === "cmi.interactions.0.correct_responses._count") {
		//	this.LMSLastErrorCode = 404;
		//	this.LMSLastErrorMsg = "Element is write only";
		//}
		else if (paramName === "cmi.core.exit") {
			this.LMSLastErrorCode = 404;
			this.LMSLastErrorMsg = "Element is write only";
		}	
		else if (paramName === "cmi.core.session_time") {
			this.LMSLastErrorCode = 404;
			this.LMSLastErrorMsg = "Element is write only";
		}	
		else {
			this.LMSLastErrorCode = 401;
			this.LMSLastErrorMsg = "Not implemented error";
		}

		this.logStatement('LMSGetValue("' + paramName + '")', result);

		return result.toString(); // The specification specifies a string return value
	}

	LMSSetValue (paramName: string, paramValue: string) {

		this.LMSLastErrorCode = 0;
		this.LMSLastErrorMsg = "";

		let result = false;

		// set the value in the sco

		if (paramName === "cmi.core._children") {
			this.LMSLastErrorCode = 402;
			this.LMSLastErrorMsg = "Invalid set value, element is a keyword";
		}
		else if (paramName === "cmi.core.student_name") {
			this.LMSLastErrorCode = 403;
			this.LMSLastErrorMsg = "Element is read only";
		}
		else if (paramName === "cmi.core.credit") {
			this.LMSLastErrorCode = 403;
			this.LMSLastErrorMsg = "Element is read only";
		}
		else if (paramName === "cmi.core.entry") {
			this.LMSLastErrorCode = 403;
			this.LMSLastErrorMsg = "Element is read only";
		}
		else if (paramName === "cmi.launch_data") {
			this.LMSLastErrorCode = 403;
			this.LMSLastErrorMsg = "Element is read only";
		}
		else if (paramName === "cmi.core.score._children") {
			this.LMSLastErrorCode = 402;
			this.LMSLastErrorMsg = "Invalid set value, element is a keyword";
		}
		else if (paramName === "cmi.student_data._children") {
			this.LMSLastErrorCode = 402;
			this.LMSLastErrorMsg = "Invalid set value, element is a keyword";
		}
		else if (paramName === "cmi.core.lesson_location") {
			if (this.isValidateDataType(paramValue, CMIDataType.CMIString255)) {
				this.sco.lessonLocation = paramValue;
				result = true;
			}
		}
		else if (paramName === "cmi.core.lesson_mode") {
			this.LMSLastErrorCode = 403;
			this.LMSLastErrorMsg = "Element is read only";
		}
		else if (paramName === "cmi.core.lesson_status") {
			if (this.isValidateDataType(paramValue, CMIDataType.Status)) {
				this.sco.lessonStatus = paramValue;
				result = true;
			}
		}
		else if (paramName === "cmi.core.score.raw") {
			if (this.isValidateDataType(paramValue, CMIDataType.CMIDecimalOrBlank)) {
				this.sco.scoreRaw = paramValue;
				result = true;
			}
		}
		else if (paramName === "cmi.core.score.min") {
			if (this.isValidateDataType(paramValue, CMIDataType.CMIDecimalOrBlank)) {
				this.sco.scoreMin = paramValue;
				result = true;
			}
		}
		else if (paramName === "cmi.core.score.max") {
			if (this.isValidateDataType(paramValue, CMIDataType.CMIDecimalOrBlank)) {
				this.sco.scoreMax = paramValue;
				result = true;
			}
		}
		else if (paramName === "cmi.core.exit") {
			if (this.isValidateDataType(paramValue, CMIDataType.Exit)) {
				this.sco.exit = paramValue;
				result = true;
            }
		}
		else if (paramName === "cmi.core.session_time") {
			if (this.isValidateDataType(paramValue, CMIDataType.CMITimeSpan)) {
				this.sco.sessionTime = paramValue;
				result = true;
			}
		}
		else if (paramName === "cmi.suspend_data") {
			if (this.isValidateDataType(paramValue, CMIDataType.CMIString4096)) {
				this.sco.suspendData = paramValue;
				result = true;
			}
		}
		else if (paramName === "cmi.comments") {
			if (this.isValidateDataType(paramValue, CMIDataType.CMIString4096)) {
				this.sco.comments = paramValue;
				result = true;
			}
		}
		else if (paramName === "cmi.objectives._children") {
			this.LMSLastErrorCode = 402;
			this.LMSLastErrorMsg = "Invalid set value, element is a keyword";
		}
		else if (paramName === "cmi.objectives._count") {
			this.LMSLastErrorCode = 402;
			this.LMSLastErrorMsg = "Invalid set value, element is a keyword";
		}
		else if (paramName.indexOf("cmi.objectives.") !== -1) {
			// get sequence number and property type from the objectives string,
			// e.g. cmi.objectives.0.id
			const segments = paramName.split(".");

			const seqNumber: number = +segments[2];
			const propertyType = segments[3];

			// if no objections exists in this position, add it.
			if (this.sco.objectives === undefined) {
				this.sco.objectives = [];
			}

			if (seqNumber > this.sco.objectives.length - 1) {
				this.sco.objectives.push({
					sequenceNumber: seqNumber,
					scormActivityObjectiveId: 0,
					id: '',
					scoreRaw: '',
					scoreMax: '',
					scoreMin: '',
					status: ''
				});
			}

			this.sco.objectives.sort(this.sortBySeqNum);

			// get the objective from the collection
			const objective = this.sco.objectives[seqNumber];

			switch (propertyType) {
				case "id":
					if (this.isValidateDataType(paramValue, CMIDataType.CMIIdentifier)) {
						objective.id = paramValue;
						result = true;
					}
					break;
				case "score":
					switch (segments[4]) {
						case "_children":
							this.LMSLastErrorCode = 402;
							this.LMSLastErrorMsg = "Invalid set value, element is a keyword";
							break;
						case "raw":
							if (this.isValidateDataType(paramValue, CMIDataType.CMIDecimalOrBlank)) {
								objective.scoreRaw = paramValue;
								result = true;
							}
							break;
						case "max":
							if (this.isValidateDataType(paramValue, CMIDataType.CMIDecimalOrBlank)) {
								objective.scoreMax = paramValue;
								result = true;
							}
							break;
						case "min":
							if (this.isValidateDataType(paramValue, CMIDataType.CMIDecimalOrBlank)) {
								objective.scoreMin = paramValue;
								result = true;
							}
							break;
					}
					break;
				case "status":
					if (this.isValidateDataType(paramValue, CMIDataType.Status)) {
						objective.status = paramValue;
						result = true;
					}
					break;
			}
			this.sco.objectives[seqNumber] = objective;

			this.sco.objectivesCount = this.sco.objectives.length;
		}
		else if (paramName === "cmi.student_preference._children") {
			this.LMSLastErrorCode = 402;
			this.LMSLastErrorMsg = "Invalid set value, element is a keyword";
		}
		else if (paramName === "cmi.student_preference.audio") {
			if (this.isValidateDataType(paramValue, CMIDataType.CMISInteger)) {
				this.sco.studentPreferenceAudio = paramValue;
				result = true;
			}
		}
		else if (paramName === "cmi.student_preference.language") {
			if (this.isValidateDataType(paramValue, CMIDataType.CMIString255)) {
				this.sco.studentPreferenceLanguage = paramValue;
				result = true;
			}
		}
		else if (paramName === "cmi.student_preference.speed") {
			if (this.isValidateDataType(paramValue, CMIDataType.CMISInteger)) {
				this.sco.studentPreferenceSpeed = paramValue;
				result = true;
			}
		}
		else if (paramName === "cmi.student_preference.text") {
			if (this.isValidateDataType(paramValue, CMIDataType.CMISInteger)) {
				this.sco.studentPreferenceText = paramValue;
				result = true;
			}
		}
		else if (paramName === "cmi.interactions._children") {
			this.LMSLastErrorCode = 402;
			this.LMSLastErrorMsg = "Invalid set value, element is a keyword";
		}
		else if (paramName === "cmi.interactions._count") {
			this.LMSLastErrorCode = 402;
			this.LMSLastErrorMsg = "Invalid set value, element is a keyword";
		}
		else if (paramName.indexOf("cmi.interactions.") != -1) {
			// get sequence number and property type from the interactions string,
			// e.g. cmi.interactions.0.id
			const segments = paramName.split(".");

			const seqNumber: number = +segments[2];
			const propertyType = segments[3];

			// if no interaction exists in this position, add it.
			if (this.sco.interactions === undefined) {
				this.sco.interactions = [];
			}

			if (seqNumber > this.sco.interactions.length - 1) {
				this.sco.interactions.push({
					sequenceNumber: seqNumber,
					id: '',
					scormActivityInteractionId: 0,
					time: '',
					type: '',
					correctResponses: [],
					weighting: '',
					studentResponse: '',
					result: '',
					latency: '',
					objectives: []
				});
			}

			this.sco.interactions.sort(this.sortBySeqNum);

			// get the interaction from the collection
			const interaction = this.sco.interactions[seqNumber];

			switch (propertyType) {
				case "id":
					if (this.isValidateDataType(paramValue, CMIDataType.CMIIdentifier)) {
						interaction.id = paramValue;
						result = true;
					}
					break;
				case "time":
					if (this.isValidateDataType(paramValue, CMIDataType.CMITime)) {
						interaction.time = paramValue;
						result = true;
					}
					break;
				case "type":
					if (this.isValidateDataType(paramValue, CMIDataType.Interaction)) {
						interaction.type = paramValue;
						result = true;
					}
					break;
				case "weighting":
					if (this.isValidateDataType(paramValue, CMIDataType.CMIDecimal)) {
						interaction.weighting = paramValue;
						result = true;
					}
					break;
				case "student_response":
					if (interaction.type === "" || this.isValidCMIFeedback(paramValue, interaction.type)) {
						interaction.studentResponse = paramValue;
						result = true;
					}
					break;
				case "result":
					if (this.isValidateDataType(paramValue, CMIDataType.Result)) {
						interaction.result = paramValue;
						result = true;
					}
					break;
				case "latency":
					if (this.isValidateDataType(paramValue, CMIDataType.CMITimeSpan)) {
						interaction.latency = paramValue;
						result = true;
					}
					break;
				case "correct_responses": {
					const objSeqId = +segments[4];
					const objProp = segments[5];

					if (objSeqId > interaction.correctResponses.length - 1) {
						interaction.correctResponses.push({
							pattern: ""
						});
					}
					interaction.correctResponses.sort(this.sortBySeqNum);
					const correctResponse = interaction.correctResponses[objSeqId];

					switch (objProp) {
						case "pattern":
							if (interaction.type === "" || this.isValidCMIFeedback(paramValue, interaction.type)) {
								correctResponse.pattern = paramValue;
								result = true;
							}
							break;
						default:
							this.LMSLastErrorCode = 401;
							this.LMSLastErrorMsg = "Not implemented error";
							break;
					}
					break;
				}
				case "objectives": {

					const objSeqId = +segments[4];
					const objProp = segments[5];

					if (objSeqId > interaction.objectives.length - 1) {
						interaction.objectives.push({
							id: ''
						});
					}

					interaction.objectives.sort(this.sortBySeqNum);

					// get the interaction from the collection
					const objective = interaction.objectives[objSeqId];

                    switch (objProp) {
                        case "id":
                            objective.id = paramValue;
                            result = true;
							break;
						default:
							this.LMSLastErrorCode = 401;
							this.LMSLastErrorMsg = "Not implemented error";
							break;
                    }

					//interaction.objectives[objSeqId] = objective;

					break;
					//case "correct_responses":
					//    // each correct answer is passed individually.  We don't need these at the moment.
					//    // "cmi.interactions.0.correct_responses.0.pattern"
					//    // "cmi.interactions.0.correct_responses.1.pattern"
					//    break;
				}
            }

			this.sco.interactions[seqNumber] = interaction;
			this.sco.interactionsCount = this.sco.interactions.length;

		}
		else if (paramName === "cmi.student_data.mastery_score") {
			this.LMSLastErrorCode = 403;
			this.LMSLastErrorMsg = "Element is read only";
		}
		else if (paramName === "cmi.student_data.max_time_allowed") {
			this.LMSLastErrorCode = 403;
			this.LMSLastErrorMsg = "Element is read only";
		}
		else if (paramName === "cmi.student_data.time_limit_action") {
			this.LMSLastErrorCode = 403;
			this.LMSLastErrorMsg = "Element is read only";
		}
		else if (paramName === "cmi.comments_from_lms") {
			this.LMSLastErrorCode = 403;
			this.LMSLastErrorMsg = "Element is read only";
		}
		else {
			this.LMSLastErrorCode = 201;
			this.LMSLastErrorMsg = "Parameter '" + paramName + "' not supported!";
		}

		this.logStatement('LMSSetValue("' + paramName + '", "' + paramValue + '")', result.toString());

		return result;
	}


	LMSCommit (str: string) {
		this.LMSLastErrorCode = 0;
		this.LMSLastErrorMsg = "";

		let result = false;
		const scoJSON = JSON.stringify(this.sco);
		const formData = new FormData();
		formData.append('sco', scoJSON);

		const apiUrl = '/api/Scorm/LMSCommitObj';

		$.ajax({
			type: "POST",
			url: apiUrl,
			data: scoJSON,
			dataType: 'json',
			contentType: "application/json; charset=utf-8",
			success: function (sResult) {
				if (!sResult) {
					this.LogError("Error throw at LMS Commit from the e-LfH LMS for content : '\n\n'", '/api/Scorm/LMSCommit');
				}
				result = true;
			},
			error: function (jqHXR, textStatus, errorThrown) {

				// Attempt SendBeacon if browser did not honour request (jqHXR.status === 0)
				// - occurs on attempt to make synchronous XMLHttp call on browser dismissal (Chrome).
				// Otherwise display error alert and return false.
				if (jqHXR.status === 0 && navigator.sendBeacon !== undefined) { 
					if (navigator.sendBeacon('/api/Scorm/LMSCommit/', formData)) {
						result = true;
					}
				}
				else {
					var errorMessage = jqHXR.status + ': ' + jqHXR.statusText
					if (jqHXR.status && jqHXR.status != 401 && jqHXR.status != 403) {

						this.LMSLastErrorCode = 101;
						this.LMSLastErrorMsg = "LMSCommit failed. Unable to communicate with the LMS.";

						alert("Error communicating with the LMS server.  Your learning details may not have been saved.\n\n" +
							"If the error persists please email support@learninghub.nhs.uk for further assistance.");
					}

					result = false;
				}
			},
			async: false
		});

		this.logStatement('LMSCommit("' + str + '")', result.toString());

		return result;
	}

	private SecondsToTime(totalSeconds: number) : string {

		const intHours = Math.floor(totalSeconds / (60 * 60));
		const intMinutes = Math.floor((totalSeconds - (intHours * 60 * 60)) / 60);
		const intSeconds = Math.floor((totalSeconds - (intHours * 60 * 60) - (intMinutes * 60)));

		const hours = intHours.toString();
		const minutes = ("00" + intMinutes.toString()).substring(intMinutes.toString().length);
		const seconds = ("00" + intSeconds.toString()).substring(intSeconds.toString().length);

		return hours + ':' + minutes + ':' + seconds;

	}

	private TimeToSeconds(sessionTime: string) : number {
		if (sessionTime === "") {
			return 0;
		} else {
			const pos1 = sessionTime.indexOf(':') + 1;
			const pos2 = sessionTime.indexOf(':', pos1 + 1) + 1;
			let pos3 = sessionTime.indexOf('.', pos2 + 1) + 1;

			if (pos3 === 0) {
				pos3 = sessionTime.length + 1;
			}

			const Hours = sessionTime.substring(0, pos1 - 1);
			const Minutes = sessionTime.substring(pos1, pos2 - 1);
			const Seconds = sessionTime.substring(pos2, pos3 - 1);
			const intHours = parseInt(Hours);
			const intMinutes = parseInt(Minutes);
			const intSeconds = parseInt(Seconds);

			const totalSeconds = (intHours * 60 * 60) + (intMinutes * 60) + intSeconds;

			return totalSeconds;
		}
	}

	private sortBySeqNum(a: any, b: any) : number {
		if (parseInt(a.SequenceNumber, 10) < parseInt(b.SequenceNumber, 10))
			return -1;
		if (parseInt(a.SequenceNumber, 10) > parseInt(b.SequenceNumber, 10))
			return 1;
		return 0;
	}

	private LogError(Message: string, URL: string) : void {

		const data = { Message: Message, URL: URL };

		console.log('Error: Message' + Message + ', URL: ' + URL);

	}

	private logStatement(statement: string, result: string) : void {
		if (this.loggingEnabled) {
			this.trace.push(
				{
					statement: statement,
					result: result
                }
			);
        }
    }

	private isValidCMIFeedback(value: string, interactionType: string): boolean {
		let response = false;
		value = value.toString();

		switch (interactionType) {
			case CMIVocabularyInteraction.TrueFalse:	// 0,1,t,f
				if (value.match(/^(0|1|t|f|true|false)$/i)) {
					response = true;
				}
				break;
			case CMIVocabularyInteraction.Choice:		// Up to 26 single chars separated by comma
				if (value.match(/^[\w]([,][\w]){0,25}$/)) { 
					response = true;
				}
				break;
			case CMIVocabularyInteraction.FillIn:		// One or more characters
				if (value.match(/^(.|\r\n|\r|\n)+$/)) {
					response = true;
				}
				break;
			case CMIVocabularyInteraction.Numeric:		// Single number, may or may not have decimal part
				if (value.match(/^\d+(?:[.,]\d+)?$/)) {
					response = true;
				}
				break;
			case CMIVocabularyInteraction.Likert:		// Zero or more of any characters
				if (value.match(/^(.|\r\n|\r|\n)*$/)) {
					response = true;
				}
				break;
			case CMIVocabularyInteraction.Matching:		// Pairs of identifiers separated by a period e.g. 1.c, 2.b, 3.a, 4.d
				if (value.match(/^([\w][.][\w])([,][\w][.][\w])*$/)) {
					response = true;
				}
				break;
			case CMIVocabularyInteraction.Performance:	// Alphanumeric limited to 255 characters
				if (value.match(/^(.{1,255})$/)) {
					response = true;
				}
				break;
			case CMIVocabularyInteraction.Sequencing:	// Sequence of elements
				if (value.match(/^\w+([,]\w+)*$/)) {
					response = true;
				}
				break;
			default:
				response = true;	// Only return false when the interaction type can be identified.
		}

		if (!response) {
			this.LMSLastErrorCode = 405;
			this.LMSLastErrorMsg = "Incorrect Data Type";
		}

		return response;
	}

	private isValidateDataType(value: string, type: CMIDataType): boolean {
		let response = false;
		value = value.toString();

        switch (type) {
			case CMIDataType.CMIBlank:
				if (value === '') {
					response = true;
                }
				break;
			case CMIDataType.CMIBoolean:
				if (value === 'true' || value === 'false') {
					response = true;
				}
				break;
			case CMIDataType.CMIDecimal: {
				if (value.match(/^-?\d+\.?\d*$/)) {
					response = true;
                }
				break;
            }
			case CMIDataType.CMIDecimalOrBlank: {
				if (value.match(/^-?\d*\.?\d*$/)) {
					response = true;
				}
				break;
			}
			case CMIDataType.CMIFeedback:
				// TODO This is dependant on the type of interacton
				response = true;
				break;
			case CMIDataType.CMIIdentifier:
				if (value.length < 256 && value.indexOf(' ') === -1) {
					response = true;
				}
				break;
			case CMIDataType.CMIInteger: {
				const result = parseInt(value);
				if (!isNaN(result) && result > -1 && result < 65537) {
					response = true;
				}
				break;
			}
			case CMIDataType.CMISInteger: {
				const result = parseInt(value);
				if (!isNaN(result) && result > -32769 && result < 32769) {
					response = true;
				}
				break;
			}
			case CMIDataType.CMIString255:
				if (value.length < 256) {
					response = true;
				}
				break;
			case CMIDataType.CMIString4096:
				if (value.length < 4096) {
					response = true;
				}
				break;
			case CMIDataType.CMITime:
				if (value.match(/^\d{2}:\d{2}:\d{2}(.\d{1,2})?$/)) {
					response = true;
				}
				break;
			case CMIDataType.CMITimeSpan:
				if (value.match(/^\d{2,4}:\d{2}:\d{2}(.\d{1,2})?$/)) {
					response = true;
				}
				break;
			case CMIDataType.Mode:
				if (value === CMIVocabularyMode.Browse
					|| value === CMIVocabularyMode.Normal
					|| value === CMIVocabularyMode.Review) {
					response = true;
				}
				break;
			case CMIDataType.Status:
				if (value === CMIVocabularyStatus.Passed
					|| value === CMIVocabularyStatus.Completed
					|| value === CMIVocabularyStatus.Failed
					|| value === CMIVocabularyStatus.Incomplete
					|| value === CMIVocabularyStatus.Browsed
					|| value === CMIVocabularyStatus.NotAttempted) {
					response = true;
				}
				break;
			case CMIDataType.Exit:
				if (value === CMIVocabularyExit.Empty
					|| value === CMIVocabularyExit.Logout
					|| value === CMIVocabularyExit.Suspend
					|| value === CMIVocabularyExit.TimeOut) {
					response = true;
                }
				break;
			case CMIDataType.Credit:
				if (value === CMIVocabularyCredit.Credit
					|| value === CMIVocabularyCredit.NoCredit) {
					response = true;
				}
				break;
			case CMIDataType.Entry:
				if (value === CMIVocabularyEntry.Empty
					|| value === CMIVocabularyEntry.AbInitio
					|| value === CMIVocabularyEntry.Resume) {
					response = true;
				}
				break;
			case CMIDataType.Interaction:
				if (value === CMIVocabularyInteraction.TrueFalse
					|| value === CMIVocabularyInteraction.Choice
					|| value === CMIVocabularyInteraction.FillIn
					|| value === CMIVocabularyInteraction.Matching
					|| value === CMIVocabularyInteraction.Performance
					|| value === CMIVocabularyInteraction.Likert
					|| value === CMIVocabularyInteraction.Sequencing
					|| value === CMIVocabularyInteraction.Numeric) {
					response = true;
				}
				break;
			case CMIDataType.Result:
				if (value === CMIVocabularyResult.Correct
					|| value === CMIVocabularyResult.Wrong
					|| value === CMIVocabularyResult.Unanticipated
					|| value === CMIVocabularyResult.Neutral) {
					response = true;
				}
				if (!response && value.match(/^-?\d+\.?\d*$/)) {
					response = true;
				}
				break;
			case CMIDataType.TimeLimitAction:
				if (value === CMIVocabularyTimeLimitAction.ContinueMessage
					|| value === CMIVocabularyTimeLimitAction.ContinueNoMessage
					|| value === CMIVocabularyTimeLimitAction.ExitMessage
					|| value === CMIVocabularyTimeLimitAction.ExitNoMessage) {
					response = true;
				}
				break;
            default:
        }

		if (!response) {
			this.LMSLastErrorCode = 405;
			this.LMSLastErrorMsg = "Incorrect Data Type";
		}

		return response;
	}
}
