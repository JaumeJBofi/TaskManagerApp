export enum TASK_STATUS {
    PENDING,
    INPROGRESS,
    COMPLETED
}

export interface CreateTaskDto{
    title: string;
    description: string;
    status: number;    
    dueDate: Date
}
export interface UpdateTaskDto{
    id: string
    title: string;
    description: string;
    status: number;    
    dueDate: Date
}


export interface UserTaskDto {    
    id: string
    title: string;
    description: string;
    status: TASK_STATUS;    
    dueDate: Date;
  }
  