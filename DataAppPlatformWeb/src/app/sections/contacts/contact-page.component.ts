import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { EntityPropertySchema } from '../../controls/common/entity-property-schema';
import { DataService } from '../../services/data.service';
import { Router, ActivatedRoute } from '@angular/router';
import { EntityDataRequest } from '../../controls/common/entity-data-request.model';
import { PageViewModel } from '../../controls/edit-page/page-view-model.model';
import { BasePageComponent } from '../../controls/edit-page/base-edit-page.component';
import { EntityDataUpdateRequest } from '../../controls/edit-page/entity-data-update-request.model';

@Component({
    selector: 'app-contact-page',
    templateUrl: './contact-page.component.html',
    styleUrls: ['./contact-page.component.css']
})
export class ContactPageComponent extends BasePageComponent {
    constructor(
        dataService: DataService,
        router: Router,
        route: ActivatedRoute
    ) {
        super(dataService, router, route);
    }

    getViewModel(): PageViewModel {
        return new PageViewModel(
            'Contacts',
            {
                'FirstName': {
                    label: 'First name',
                    required: true,
                    visible: true,
                    enabled: true
                },
                'LastName': {
                    label: 'Last name',
                    required: true,
                    visible: true,
                    enabled: true
                }
            }
        );
    }
}
