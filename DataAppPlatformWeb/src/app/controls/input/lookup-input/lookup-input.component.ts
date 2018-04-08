import { Component, Input, Output, EventEmitter, OnInit, ViewChild, ElementRef, Renderer } from '@angular/core';
import { DataService } from '../../../core.module';
import { EntityPropertySchema } from '../../edit-page/entity-property-schema';
import { LookupAutoCompleteListItem } from '../../../core.module/api/data-service/lookup-autocomplete-list-item.model';
import { LookupAutoCompleteRequest } from '../../../core.module/api/data-service/lookup-autocomplete-request.model';

@Component({
    selector: 'app-lookup-input',
    templateUrl: './lookup-input.component.html'
})
export class LookupInputComponent implements OnInit {

    @ViewChild('textInput') input: ElementRef;

    constructor(private dataService: DataService, private renderer: Renderer) { }

    @Output() modelChange: EventEmitter<EntityPropertySchema> = new EventEmitter<EntityPropertySchema>();

    private innerModel: EntityPropertySchema;

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

    public myFocusTriggeringEventEmitter = new EventEmitter<boolean>();

    private inputValue: string;
    private editMode: boolean;
    private inputFocused: boolean;
    private suggestionsVisible: boolean;
    private selectedValue: LookupAutoCompleteListItem;
    private suggestions: LookupAutoCompleteListItem[];
    private errorMessage: any = '';
    private linkClicked: boolean; // hook to stop switchToEditMode()

    ngOnInit() { }

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
        if (this.innerModel.value == null) {
            this.innerModel.displayValue = '';
            this.suggestionsVisible = false;
        } else {
            this.editMode = false;
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

    private getSuggestionsMargin() {
        const margin = this.suggestions ? ((this.suggestions.length - 1) * 20) + 41 : 0;
        return margin;
    }

    public inputKeyUp(event: KeyboardEvent) {
        if (this.innerModel.displayValue && this.innerModel.displayValue.length === 0) {
            this.suggestionsVisible = false;
            this.setSelectedSuggestion(null);
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
        this.setInputFocused();
    }

    private findLookupValues(search: string) {
        this.dataService.getAutocompleteData(new LookupAutoCompleteRequest(this.innerModel.schemaName, search, null))
        .subscribe(data => {
            if (data) {
                this.selectedValue = null;
            }
            this.suggestions = data;
        });
    }
}
