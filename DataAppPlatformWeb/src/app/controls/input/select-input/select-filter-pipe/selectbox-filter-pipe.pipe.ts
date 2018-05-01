import { Pipe, PipeTransform } from '@angular/core';
import { SelectItem } from '../models/select-item.model';

@Pipe({
    name: 'listboxFilter',
    pure: false
})
export class SelectFilterPipe implements PipeTransform {
    transform(items: SelectItem[], filter: any): any {
        if (!items || !filter) {
            return items;
        }
        return items.filter(item => item.name.indexOf(filter) !== -1);
    }
}
