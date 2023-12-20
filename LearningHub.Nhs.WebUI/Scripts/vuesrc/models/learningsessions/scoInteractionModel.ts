export class ScoInteractionModel {
	sequenceNumber: number;
	scormActivityInteractionId: number;
	id: string;
	time: string;
	type: string;
	result: string;
	studentResponse: string;
	weighting: string;
	latency: string;
	correctResponses: CorrectResponseModel[];
	objectives: InteractionObjectiveModel[];
}

export class CorrectResponseModel {
	pattern: string;
}

export class InteractionObjectiveModel {
	id: string;
}