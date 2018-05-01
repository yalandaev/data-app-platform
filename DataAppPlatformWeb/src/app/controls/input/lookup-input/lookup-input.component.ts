import { Component, Input, Output, EventEmitter, OnInit, ViewChild, ElementRef, Renderer } from '@angular/core';
import { DataService } from '../../../core.module';
import { EntityPropertySchema } from '../../edit-page/entity-property-schema';
import { LookupAutoCompleteListItem } from '../../../core.module/api/data-service/lookup-autocomplete-list-item.model';
import { LookupAutoCompleteRequest } from '../../../core.module/api/data-service/lookup-autocomplete-request.model';
import { LookupMode } from './lookup-mode.model';
import { EditPageMode } from '../../edit-page/base-edit-page.component';

@Component({
    selector: 'app-lookup-input',
    templateUrl: './lookup-input.component.html'
})
export class LookupInputComponent implements OnInit {
    constructor(
        private dataService: DataService,
        private renderer: Renderer) {
    }

    private innerModel: EntityPropertySchema;
    private editMode: boolean;
    private inputFocused: boolean;
    private suggestionsVisible: boolean;
    private selectedValue: LookupAutoCompleteListItem;
    private suggestions: LookupAutoCompleteListItem[];
    private errorMessage: any = '';
    private linkClicked: boolean; // hook to stop switchToEditMode()
    private lookupMode: LookupMode;

    @ViewChild('textInput')
    input: ElementRef;

    @Output()
    modelChange: EventEmitter<EntityPropertySchema> = new EventEmitter<EntityPropertySchema>();

    @Input()
    set model(model: EntityPropertySchema) {
        this.innerModel = model;
        if (model.value && model.displayValue) {
            this.selectedValue = {
                Id: model.value,
                DisplayValue: model.displayValue
            };
        }
        this.editMode = this.selectedValue ? false : true;
    }

    @Input()
    mode: string;

    ngOnInit() {
        if (this.mode === 'list') {
            this.lookupMode = LookupMode.List;
        } else {
            this.lookupMode = LookupMode.Lookup;
        }
     }

    arrowDownClicked() {
        this.findLookupValues('');
        this.editMode = true;
    }

    getEditMode() {
        if (this.selectedValue == null) {
            this.editMode = true;
        }
    }

    private setInputFocused() {
        const scope = this;
        setTimeout(function() {
            scope.renderer.invokeElementMethod(scope.input.nativeElement, 'focus');
        }, 300);
    }

    private onFocusOut() {
        this.suggestionsVisible = false;
        if (this.innerModel.value == null || this.innerModel.value === '') {
            this.innerModel.displayValue = '';
        } else {
            this.editMode = false;
            if (this.selectedValue) {
                this.innerModel.displayValue = this.selectedValue.DisplayValue;
            }
        }
    }

    private clickOnLink(event) {
        this.linkClicked = true;
        event.stopPropagation();
        event.preventDefault();
    }

    private clear() {
        this.selectedValue = null;
        this.innerModel.value = null;
        this.innerModel.displayValue = null;
        this.modelChange.emit(this.innerModel);
        this.editMode = true;
    }

    public inputKeyUp(event: KeyboardEvent) {
        if (this.innerModel.displayValue === '' && this.innerModel.displayValue.length === 0) {
            if (this.lookupMode === LookupMode.Lookup) {
                this.suggestionsVisible = false;
                this.setSelectedSuggestion(null);
            }
            if (this.lookupMode === LookupMode.List) {
                this.findLookupValues('');
                this.editMode = true;
            }
        }
        if (this.innerModel.displayValue && this.innerModel.displayValue.length > 0) {
            this.suggestionsVisible = true;
            this.findLookupValues(this.innerModel.displayValue);
        }
    }

    public setSelectedSuggestion(suggestion: LookupAutoCompleteListItem) {
        this.selectedValue = suggestion;
        this.innerModel.displayValue = suggestion == null ? '' : suggestion.DisplayValue;
        this.innerModel.value = suggestion == null ? '' : suggestion.Id;
        this.editMode = suggestion == null;
        this.suggestionsVisible = false;
        this.modelChange.emit(this.innerModel);
    }

    public suggestionMouseDown(suggestion: LookupAutoCompleteListItem) {
        this.setSelectedSuggestion(suggestion);
    }

    private switchToEditMode() {
        if (this.linkClicked) {
            this.linkClicked = false;
            return;
        }
        this.editMode = true;
        if (this.lookupMode === LookupMode.List) {
            this.findLookupValues('');
        }
        this.setInputFocused();
    }

    private findLookupValues(search: string) {
        this.dataService.getAutocompleteData(new LookupAutoCompleteRequest(this.innerModel.schemaName, search, this.innerModel.filter))
        .subscribe(data => {
            this.suggestions = data;
            if (this.lookupMode === LookupMode.List) {
                this.suggestionsVisible = true;
                this.setInputFocused();
            }
        });
    }

    private onInputClick() {
        if (this.lookupMode === LookupMode.List) {
            if (this.lookupMode === LookupMode.List) {
                this.findLookupValues('');
            }
            this.setInputFocused();
        }
    }

    private isLookupMode(): boolean {
        return this.lookupMode === LookupMode.Lookup;
    }
    private isListMode(): boolean {
        return this.lookupMode === LookupMode.List;
    }
}
