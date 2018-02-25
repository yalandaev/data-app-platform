import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { DataRequest } from '../controls/data-table/models/dataRequest.model';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class DataService {

  url = 'http://localhost:4300/api/DataService/GetData';

  constructor(private http: HttpClient) { }

  getData(request: DataRequest): Observable<any> {
    return this.http.post(this.url, request);
  }
}
