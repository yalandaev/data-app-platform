import { PageViewModel } from './page-view-model.model';
import { Router, ActivatedRoute } from '@angular/router';
import { DataService, EntityDataChangeRequest, EntityDataQueryRequest } from '../../core.module';

export enum EditPageMode {
    New,
    Edit
}

export abstract class BasePageComponent {
    public viewModel: PageViewModel;
    protected mode: EditPageMode;

    constructor(
        private dataService: DataService,
        protected router: Router,
        protected route: ActivatedRoute
    ) {
        this.route.params.subscribe( params => {
            const idParam = params['id'];
            this.mode = idParam === 'new' ? EditPageMode.New : EditPageMode.Edit;
            this.viewModel = this.getViewModel();
            if (this.mode === EditPageMode.Edit) {
                this.viewModel.entityId = idParam;
                this.getData();
            }
        });
    }

    abstract getViewModel(): PageViewModel;
    abstract close(): void;

    protected getQueryColumns() {
        return Object.keys(this.viewModel.fields);
    }

    protected getOutputData(forUpdate: boolean = false) {
        const data = {};
        Object.keys(this.viewModel.fields).forEach(key => {
            if (!forUpdate || (forUpdate && this.viewModel.fields[key].value !== this.viewModel.fields[key].oldValue)) {
                data[key] = {
                    value: this.viewModel.fields[key].value,
                    oldValue: this.viewModel.fields[key].oldValue
                };
            }
        });
        return data;
    }

    protected save() {
        const updateRequest = new EntityDataChangeRequest(this.viewModel.entitySchema, this.getOutputData(true), this.viewModel.entityId);
        if (this.mode === EditPageMode.Edit) {
            this.dataService.setEntityData(updateRequest).subscribe(response => { console.log(response); });
        } else {
            this.dataService.createEntityData(updateRequest).subscribe(response => { console.log(response); });
        }
    }

    protected getData() {
        this.dataService.getEntityData(
            new EntityDataQueryRequest(this.viewModel.entitySchema, this.viewModel.entityId, this.getQueryColumns()))
            .subscribe(response => {
                const data = response.Fields;
                Object.keys(this.viewModel.fields).forEach(key => {
                    this.viewModel.fields[key].value = data[key].Value;
                    this.viewModel.fields[key].oldValue = data[key].Value;
                    this.viewModel.fields[key].displayValue = data[key].DisplayValue;
                });
            });
    }
}
