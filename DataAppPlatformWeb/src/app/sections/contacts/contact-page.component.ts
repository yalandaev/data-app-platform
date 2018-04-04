import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { EntityPropertySchema } from '../../controls/common/entity-property-schema';
import { DataService } from '../../services/data.service';
import { Router, ActivatedRoute } from '@angular/router';
import { EntityDataRequest } from '../../controls/common/entity-data-request.model';


@Component({
    selector: 'app-contact-page',
    templateUrl: './contact-page.component.html',
    styleUrls: ['./contact-page.component.css']
})
export class ContactPageComponent implements OnInit {
    readonly viewModel: IDictionary<EntityPropertySchema>;
    selected: boolean;
    entitySchema: string;
    entityId: number;

    constructor(
        private dataService: DataService,
        private router: Router,
        private route: ActivatedRoute
    ) {
        this.entitySchema = 'Contacts';
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

        this.route.params.subscribe( params => {
            this.entityId = params['id'];
            this.getData();
        });
    }

    ngOnInit() { }

    getData() {
        this.dataService.getEntityData(
            new EntityDataRequest(this.entitySchema, this.entityId, this.getQueryColumns()))
            .subscribe(response => {
                const data = response.Fields;
                Object.keys(this.viewModel).forEach(key => {
                    this.viewModel[key].value = data[key].Value;
                    this.viewModel[key].oldValue = data[key].Value;
                    this.viewModel[key].displayValue = data[key].DisplayValue;
                });
            });
    }

    private getOutputData(forUpdate: boolean = false) {
        const data = {};
        Object.keys(this.viewModel).forEach(key => {
            if (!forUpdate || (forUpdate && this.viewModel[key].value !== this.viewModel[key].oldValue)) {
                data[key] = {
                    value: this.viewModel[key].value,
                    oldValue: this.viewModel[key].oldValue
                };
            }
        });
        return data;
    }

    private getQueryColumns() {
        return Object.keys(this.viewModel);
    }

    private setValue() {
        this.viewModel['LastName'].value = 'Changed from parent';
    }
}

export interface IDictionary<T> {
    [details: string]: T;
}
