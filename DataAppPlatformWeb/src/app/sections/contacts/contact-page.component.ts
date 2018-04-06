import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { PageViewModel } from '../../controls/edit-page/page-view-model.model';
import { BasePageComponent } from '../../controls/edit-page/base-edit-page.component';
import { DataService } from '../../core.module';

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
