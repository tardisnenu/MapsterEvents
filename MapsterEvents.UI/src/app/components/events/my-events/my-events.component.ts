import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { EventService } from '../../../services/event.service';
import { EventListItem } from '../../../models/event.model';

/**
 * My Events Component
 * Created by Hamza Canturk - https://hamzacanturk.com/
 */
@Component({
  selector: 'app-my-events',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="container">
      <div class="row">
        <div class="col-12">
          <h2 class="mb-4">Organize Ettiğim Etkinlikler</h2>
          <div class="row" *ngIf="events.length > 0">
            <div class="col-md-6 col-lg-4 mb-4" *ngFor="let event of events">
              <div class="card">
                <div class="card-body">
                  <h5 class="card-title">{{ event.title }}</h5>
                  <p class="card-text">{{ event.description | slice:0:100 }}...</p>
                  <div class="d-flex justify-content-between align-items-center">
                    <small class="text-muted">{{ event.date | date:'medium' }}</small>
                    <a [routerLink]="['/events', event.id]" class="btn btn-sm btn-primary">Detay</a>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div *ngIf="events.length === 0" class="text-center py-5">
            <p class="text-muted">Henüz organize ettiğiniz etkinlik bulunmamaktadır.</p>
            <a routerLink="/events/create" class="btn btn-primary">Etkinlik Oluştur</a>
          </div>
        </div>
      </div>
    </div>
  `
})
export class MyEventsComponent implements OnInit {
  events: EventListItem[] = [];

  constructor(private eventService: EventService) {}

  ngOnInit(): void {
    this.loadMyEvents();
  }

  private loadMyEvents(): void {
    this.eventService.getMyOrganizedEvents().subscribe({
      next: (events) => this.events = events,
      error: (error) => console.error('Error loading my events:', error)
    });
  }
} 