import { PageViewModel } from './page-view-model.model';
import { Router, ActivatedRoute } from '@angular/router';
import { DataService, EntityDataUpdateRequest, EntityDataRequest } from '../../core.module';

export abstract class BasePageComponent {
    public viewModel: PageViewModel;

    constructor(
        private dataService: DataService,
        private router: Router,
        private route: ActivatedRoute
    ) {
        this.route.params.subscribe( params => {
            this.viewModel = this.getViewModel();
            this.viewModel.entityId = params['id'];
            this.getData();
        });
    }

    abstract getViewModel(): PageViewModel;

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
        const updateRequest = new EntityDataUpdateRequest(this.viewModel.entitySchema, this.getOutputData(true), this.viewModel.entityId);
        this.dataService.setEntityData(updateRequest).subscribe(response => { console.log(response); });
    }

    protected getData() {
        this.dataService.getEntityData(
            new EntityDataRequest(this.viewModel.entitySchema, this.viewModel.entityId, this.getQueryColumns()))
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
