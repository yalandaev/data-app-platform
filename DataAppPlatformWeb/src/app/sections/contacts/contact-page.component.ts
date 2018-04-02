import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { EntityPropertySchema } from '../../controls/common/entity-property-schema';


@Component({
  selector: 'app-contact-page',
  templateUrl: './contact-page.component.html',
  styleUrls: ['./contact-page.component.css']
})
export class ContactPageComponent implements OnInit {
  data = null;
  model = {};
  viewModel: {
      fname: EntityPropertySchema,
      lname: EntityPropertySchema
  };
  propertySchema: EntityPropertySchema;

  constructor() {
      this.viewModel = {
          fname: {
              label: 'First name',
              required: true,
              visible: true,
              enabled: true
          },
          lname: {
            label: 'Last name',
            required: true,
            visible: true,
            enabled: true
        }
      };
  }

  ngOnInit() { }
}
