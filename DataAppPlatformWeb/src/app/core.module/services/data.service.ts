import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';
import { DataResponse } from '../api/data-service/data-response.model';
import { DataRequest } from '../api/data-service/data-request.model';
import { EntityDataRequest } from '../api/data-service/entity-data-request.model';
import { EntityDataUpdateRequest } from '../api/data-service/entity-data-update-request.model';

@Injectable()
export class DataService {

  url = 'http://localhost:4300/api/DataService/';

  constructor(private http: HttpClient) { }

  getData(request: DataRequest): Observable<DataResponse> {
    return this.http.post<DataResponse>(this.url + 'GetData', request);
  }

  getEntityData(request: EntityDataRequest): Observable<any> {
    return this.http.post(this.url + 'GetEntityData', request);
  }

  setEntityData(request: EntityDataUpdateRequest): Observable<any> {
    return this.http.post(this.url + 'SetEntityData', request);
  }
}
