import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';
import { DataResponse } from '../api/data-service/data-response.model';

import { EntityDataQueryRequest } from '../api/data-service/entity-data-query-request.model';
import { DataQueryRequest } from '../api/data-service/data-query-request.model';
import { EntityDataChangeRequest } from '../api/data-service/entity-data-change-request.model';
import { LookupAutoCompleteRequest } from '../api/data-service/lookup-autocomplete-request.model';
import { LookupAutoCompleteListItem } from '../api/data-service/lookup-autocomplete-list-item.model';


@Injectable()
export class DataService {

  url = 'http://localhost:4300/api/DataService/';

  constructor(private http: HttpClient) { }

  getData(request: DataQueryRequest): Observable<DataResponse> {
    return this.http.post<DataResponse>(this.url + 'GetData', request);
  }

  getEntityData(request: EntityDataQueryRequest): Observable<any> {
    return this.http.post(this.url + 'GetEntity', request);
  }

  setEntityData(request: EntityDataChangeRequest): Observable<any> {
    return this.http.post(this.url + 'SetEntity', request);
  }

  createEntityData(request: EntityDataChangeRequest): Observable<any> {
    return this.http.post(this.url + 'CreateEntity', request);
  }

  getAutocompleteData(request: LookupAutoCompleteRequest): Observable<LookupAutoCompleteListItem[]> {
    return this.http.post<LookupAutoCompleteListItem[]>(this.url + 'LookupAutoComplete', request);
  }
}
