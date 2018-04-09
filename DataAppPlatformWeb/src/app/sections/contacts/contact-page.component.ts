import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { PageViewModel } from '../../controls/edit-page/page-view-model.model';
import { BasePageComponent, EditPageMode } from '../../controls/edit-page/base-edit-page.component';
import { DataService } from '../../core.module';
import { LookupMode } from '../../controls/input/lookup-input/lookup-mode.model';

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

    lookupMode = LookupMode;

    getViewModel(): PageViewModel {
        return new PageViewModel(
            'Contacts',
            {
                'Id': {
                    label: 'Id',
                    required: true,
                    visible: true,
                    enabled: false
                },
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
                },
                'Phone': {
                    label: 'Phone',
                    required: true,
                    visible: true,
                    enabled: true
                },
                'Email': {
                    label: 'Email',
                    required: true,
                    visible: true,
                    enabled: true
                },
                'Manager': {
                    label: 'Manager',
                    required: true,
                    visible: true,
                    enabled: true,
                    schemaName: 'Contacts',
                    allowHyperlink: true
                },
                'Gender': {
                    label: 'Gender',
                    required: true,
                    visible: true,
                    enabled: true,
                    schemaName: 'Gender',
                    allowHyperlink: true
                }
            }
        );
    }

    protected save() {
        super.save();
        if (this.mode === EditPageMode.New) {
            this.router.navigate(['/contacts']);
        }
    }
    close() {
        this.router.navigate(['/contacts']);
    }
}
