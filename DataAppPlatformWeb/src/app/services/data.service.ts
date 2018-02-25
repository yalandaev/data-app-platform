import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { DataRequest } from '../controls/data-table/models/dataRequest.model';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class DataService {

  url = 'https://195b162a-315e-4ee7-ac72-ca8ecb01ca31.mock.pstmn.io/data';

  constructor(private http: HttpClient) { }

  getData(request: DataRequest): Observable<any> {
    return this.http.post(this.url, request);
  }
}
