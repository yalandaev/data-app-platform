import { Pipe, PipeTransform } from '@angular/core';
import { DataTableColumn } from '../models/column.model';
import { DatePipe } from '@angular/common';

@Pipe({name: 'useFilter'})
export class UseFilterPipe implements PipeTransform {
    transform(value: any, column: DataTableColumn): any {
        if (!column.formatter) {
            return value;
        }
        const datePipe: DatePipe = new DatePipe('en-US');
        return datePipe.transform(value);
    }
}

// TODO: Need to make something like this:
// https://stackoverflow.com/questions/21491747/apply-formatting-filter-dynamically-in-a-ng-repeat