import { Component, OnInit, ViewChild, ElementRef, Output, EventEmitter, Input, Renderer, AfterViewInit } from '@angular/core';
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
  model: string;

  @Input()
  label: string;

  @Input()
  items: SelectItem[];

  ngOnInit() {
    if (this.items && this.model) {
      this.displayValue = this.items.filter(x => x.value === this.model)[0].name;
    }
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
