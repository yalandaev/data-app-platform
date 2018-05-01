import { Component, OnInit, ViewChild, ElementRef, Output, EventEmitter, Input, Renderer } from '@angular/core';
import { SelectItem } from './models/select-item.model';


@Component({
  selector: 'app-select-input',
  templateUrl: './select-input.component.html',
  styleUrls: ['./select-input.component.css']
})
export class SelectInputComponent implements OnInit {
  displayValue: string;
  itemsVisible: boolean;
  selectedItem: SelectItem;
  filter: string;

  constructor(private renderer: Renderer) { }

  @ViewChild('textInput')
  input: ElementRef;

  @Output()
  modelChange: EventEmitter<string> = new EventEmitter<string>();

  @Input()
  set model(value: string) {

  }

  @Input()
  label: string;

  @Input()
  items: SelectItem[];

  ngOnInit() {
  }

  itemMouseDown(item: SelectItem) {
    this.selectedItem = item;
    this.displayValue = item.name;
    this.modelChange.emit(item.value);
  }

  inputKeyUp(event: KeyboardEvent) {
    this.filter = this.displayValue;
  }

  onFocusOut() {
    this.itemsVisible = false;
    this.filter = null;
  }

  onInputClick() {
    this.itemsVisible = true;
  }

  arrowDownClicked() {
    this.itemsVisible = true;
    this.setInputFocused();
  }

  private setInputFocused() {
    setTimeout(() => {
        this.renderer.invokeElementMethod(this.input.nativeElement, 'focus');
    }, 300);
  }
}
