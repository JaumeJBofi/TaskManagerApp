import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment.dev';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService]
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear(); // Clear local storage after each test
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should login successfully and store tokens', () => {
    const loginDto = { userName: 'testuser', password: 'testpassword' };
    const mockResponse: any = { 
      xsrfToken: 'test-xsrf-token',
      accessToken: 'test-access-token',
      userDto: {
        userNmae: 'testuser',
        email: 'test@example.com'
      }
    };

    service.login(loginDto).subscribe(response => {
      expect(response).toEqual(mockResponse);
      expect(localStorage.getItem('XSRF-TOKEN')).toBe('test-xsrf-token');
      expect(localStorage.getItem('ACCESS-TOKEN')).toBe('test-access-token');
    });

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/Auth/Login`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(loginDto);
    req.flush(mockResponse);
  });

  it('should signin successfully', () => {
    const signInDto = { userName: 'newuser', email: 'newuser@example.com', password: 'newpassword' };

    service.signin(signInDto).subscribe(response => {
      expect(response).toBeTruthy(); // Assuming a successful response is truthy
    });

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/Auth/SignIn`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(JSON.stringify(signInDto)); 
    req.flush({}); // You might need to adjust the response based on your API
  });

  it('should logout successfully', () => {
    service.logout().subscribe(response => {
      expect(response).toBeTruthy(); // Assuming a successful response is truthy
    });

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/Auth/Logout`);
    expect(req.request.method).toBe('POST');
    req.flush({}); // You might need to adjust the response based on your API
  });

  it('should check if user is logged in - true', () => {
    service.isLoggedIn().subscribe(loggedIn => {
      expect(loggedIn).toBeTrue();
    });

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/Auth/IsUserLogged`);
    expect(req.request.method).toBe('POST');
    req.flush({}); // Simulate a successful response from the API
  });

  it('should check if user is logged in - false', () => {
    const mockError = new ErrorEvent('Network error'); 

    service.isLoggedIn().subscribe(
      () => {}, 
      error => {
        expect(error).toBeTruthy(); 
      }
    );

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/Auth/IsUserLogged`);
    expect(req.request.method).toBe('POST');
    req.error(mockError); 
  });
});
