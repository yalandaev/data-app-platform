import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { DataRequest } from '../controls/data-table/models/data-request.model';
import { HttpClient } from '@angular/common/http';
import { EntityDataRequest } from '../controls/common/entity-data-request.model';

@Injectable()
export class DataService {

  url = 'http://localhost:4300/api/DataService/';

  constructor(private http: HttpClient) { }

  getData(request: DataRequest): Observable<any> {
    return this.http.post(this.url + 'GetData', request);
  }

  getEntityData(request: EntityDataRequest): Observable<any> {
    return this.http.post(this.url + 'GetEntityData', request);
  }
}
