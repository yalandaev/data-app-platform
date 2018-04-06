import { Injectable, ErrorHandler } from '@angular/core';

@Injectable()
export class UIErrorHandler extends ErrorHandler  {
  constructor() {
    super();
  }
  handleError(error) {
    super.handleError(error);
    alert(`Error occurred: ${error.message}`);
  }
}
