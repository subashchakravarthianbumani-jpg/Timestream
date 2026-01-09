import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CameraData } from '../_models/dashboard.model';
import { environment } from 'src/environments/environment';
import { ResponseModel } from '../_models/ResponseStatus';
import { Router } from '@angular/router';
import { httpOptions } from '../_models/utils';
import{DashboardCameraModel} from '../_models/dashboard.model';
import { map } from 'rxjs';


@Injectable({ providedIn: 'root' })
export class CameraService {

  constructor(private router: Router, private http: HttpClient) {}
  
// getAllCameras(model: DashboardCameraModel): Observable<CameraData[]> {
//   return this.http.post<ResponseModel>(
//       `${environment.apiUrl}/Dashboard/DashboardcameraLive`,
//       model,
//       httpOptions
//   ).pipe(map(res => res.data as CameraData[]));
// }

getAllCameras(model: DashboardCameraModel): Observable<ResponseModel> {
  return this.http.post<ResponseModel>(
    `${environment.apiUrl}/Dashboard/DashboardcameraLive`,
    model,
    httpOptions
  );
}


}
