import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { EntityPropertySchema } from '../../controls/common/entity-property-schema';


@Component({
    selector: 'app-contact-page',
    templateUrl: './contact-page.component.html',
    styleUrls: ['./contact-page.component.css']
})
export class ContactPageComponent implements OnInit {
    readonly viewModel: IDictionary<EntityPropertySchema>;

    constructor() {
        this.viewModel = {
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
        };

        this.getData();
    }

    ngOnInit() { }

    getData() {
        Object.keys(this.viewModel).forEach(key => {
            this.viewModel[key].value = 'sample';
            this.viewModel[key].oldValue = 'sample';
        });
    }

    private getOutputData(forUpdate: boolean = false) {
        const data = {};
        Object.keys(this.viewModel).forEach(key => {
            if (!forUpdate || (forUpdate && this.viewModel[key].value !== this.viewModel[key].oldValue)) {
                data[key] = {
                    value: this.viewModel[key].value,
                    oldValue: this.viewModel[key].oldValue,
                };
            }
        });
        return data;
    }

    private setValue() {
        this.viewModel['LastName'].value = 'Changed from parent';
        this.viewModel['LastName'].enabled = !this.viewModel['LastName'].enabled;
    }
}

export interface IDictionary<T> {
    [details: string]: T;
}
