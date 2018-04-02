import { Component, OnInit, Output, Input, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { EntityPropertySchema } from '../../common/entity-property-schema';


@Component({
    selector: 'app-text-input',
    templateUrl: './text-input.component.html',
})
export class TextInputComponent implements OnInit {

    @ViewChild('textInput') input: ElementRef;

    constructor() { }

    @Output() modelChange: EventEmitter<string> = new EventEmitter<string>();

    @Input() set model(item: string) {
        this.value = item;
    }

    @Input()
    propertySchema: EntityPropertySchema;

    private value: string;
    private errorMessage: any = '';

    onChanges(newValue) {
        this.value = newValue;
        this.modelChange.emit(newValue);
    }

    ngOnInit() { }

    private clear() {
        this.model = null;
        this.modelChange.emit(null);
    }
}
