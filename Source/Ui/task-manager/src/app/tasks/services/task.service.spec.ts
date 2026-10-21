import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing'; 
import { TaskService } from './task.service';
import { TASK_STATUS, UserTaskDto } from '../models/task';
import { environment } from '../../../environments/environment.dev';

describe('TaskService', () => {
  let service: TaskService;
  let httpMock: HttpTestingController; 

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule], 
      providers: [TaskService]
    });
    service = TestBed.inject(TaskService);
    httpMock = TestBed.inject(HttpTestingController); 
  });

  afterEach(() => {
    httpMock.verify(); 
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get tasks from the API', () => {
    const mockTasks: UserTaskDto[] = [
      { id: '1', title: 'Task 1', description: 'Description 1', status: TASK_STATUS.PENDING, dueDate: new Date() },
      { id: '2', title: 'Task 2', description: 'Description 2', status: TASK_STATUS.COMPLETED, dueDate: new Date() }
    ];

    service.getTasks().subscribe(tasks => {
      expect(tasks).toEqual(mockTasks);
    });

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/TaskManagment/GetTasks`); 
    expect(req.request.method).toBe('GET');
    req.flush(mockTasks); 
  });

  it('should create a new task', () => {
    const newTask: UserTaskDto = {
      id: '3', 
      title: 'New Task',
      description: 'New task description',
      status: TASK_STATUS.INPROGRESS,
      dueDate: new Date()
    };

    service.createTask(newTask).subscribe(task => {
      expect(task).toEqual(newTask); 
    });

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/TaskManagment/CreateTask`); 
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({title: newTask.title, description: newTask.description, dueDate: newTask.dueDate, status: newTask.status }); 
    req.flush(newTask); 
  });

  it('should update an existing task', () => {
    const updatedTask: UserTaskDto = {
      id: '1', 
      title: 'Updated Task',
      description: 'Updated task description',
      status: TASK_STATUS.COMPLETED,
      dueDate: new Date()
    };

    service.updateTask(updatedTask).subscribe(task => {
      expect(task).toEqual(updatedTask); 
    });

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/TaskManagment/UpdateTask/${updatedTask.id}`); 
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual({ id: updatedTask.id, title: updatedTask.title, description: updatedTask.description, dueDate: updatedTask.dueDate, status: updatedTask.status }); 
    req.flush(updatedTask); 
  });

  it('should delete a task', () => {
    const taskId = '1';

    service.deleteTask(taskId).subscribe(result => {
      expect(result).toBeNull(); 
    });

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/TaskManagment/DeleteTask/${taskId}`); 
    expect(req.request.method).toBe('DELETE');
    req.flush(null); 
  });

  it('should calculate due date', () => {
    const dueDate = service.calculateDueDate();
    const today = new Date();
    today.setHours(0, 0, 0, 0); 
    expect(dueDate.toDateString()).toBe(today.toDateString());
  });

  it('should display status correctly', () => {
    expect(service.displayStatus(TASK_STATUS.COMPLETED)).toBe('Completed');
    expect(service.displayStatus(TASK_STATUS.INPROGRESS)).toBe('In Progress');
    expect(service.displayStatus(TASK_STATUS.PENDING)).toBe('Pending');
    expect(service.displayStatus(1)).toBe('Unknown'); 
  });
});
