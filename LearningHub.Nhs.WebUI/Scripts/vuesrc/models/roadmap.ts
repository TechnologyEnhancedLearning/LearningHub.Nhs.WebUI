
export class RoadMapModel {
    roadmapTypeId : number;
    title: string;
    description : string;
    roadmapDate: Date;
    imageName: string;
    orderNumber : number;
    published : boolean;    
}

export class RoadMapResultModel {
    totalRecords: number;
    roadMapItems : RoadMapModel[]
}