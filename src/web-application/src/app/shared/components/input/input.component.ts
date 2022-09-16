import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-input',
  templateUrl: './input.component.html',
  styleUrls: ['./input.component.scss']
})
export class InputComponent implements OnInit {
  @Input() ipt_label!: string;
  @Input() ipt_type!: string;
  constructor() { }

  ngOnInit(): void {
  }

}
