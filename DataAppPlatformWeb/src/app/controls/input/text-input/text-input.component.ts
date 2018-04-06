import { Component, OnInit, Output, Input, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { EntityPropertySchema } from '../../edit-page/entity-property-schema';

@Component({
    selector: 'app-text-input',
    templateUrl: './text-input.component.html',
})
export class TextInputComponent implements OnInit {

    @ViewChild('textInput') input: ElementRef;

    constructor() { }

    @Output() modelChange: EventEmitter<EntityPropertySchema> = new EventEmitter<EntityPropertySchema>();

    @Input()
    model: EntityPropertySchema;

    private value: string;
    private errorMessage: any = '';

    onChanges(newValue) {
        this.model.value = newValue;
        this.modelChange.emit(this.model);
    }

    ngOnInit() { }

    private clear() {
        this.model.value = null;
        this.modelChange.emit(this.model);
    }
}
