import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { EventService } from '../../../services/event.service';
import { CategoryService } from '../../../services/category.service';
import { Category } from '../../../models/category.model';

/**
 * Event Create Component
 * Created by Hamza Canturk - https://hamzacanturk.com/
 */
@Component({
  selector: 'app-event-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-lg-8">
          <div class="card mt-4">
            <div class="card-header">
              <h2>Yeni Etkinlik Oluştur</h2>
            </div>
            <div class="card-body">
              <form [formGroup]="eventForm" (ngSubmit)="onSubmit()">
                <div class="mb-3">
                  <label class="form-label">Başlık</label>
                  <input type="text" class="form-control" formControlName="title">
                </div>
                <div class="mb-3">
                  <label class="form-label">Açıklama</label>
                  <textarea class="form-control" rows="4" formControlName="description"></textarea>
                </div>
                <div class="mb-3">
                  <label class="form-label">Tarih</label>
                  <input type="datetime-local" class="form-control" formControlName="date">
                </div>
                <div class="mb-3">
                  <label class="form-label">Konum</label>
                  <input type="text" class="form-control" formControlName="location">
                </div>
                <div class="mb-3">
                  <label class="form-label">Kategori</label>
                  <select class="form-select" formControlName="categoryId">
                    <option value="">Kategori Seçin</option>
                    <option *ngFor="let category of categories" [value]="category.id">
                      {{ category.name }}
                    </option>
                  </select>
                </div>
                <div class="d-flex gap-2">
                  <button type="submit" class="btn btn-primary" [disabled]="isLoading">
                    {{ isLoading ? 'Oluşturuluyor...' : 'Etkinlik Oluştur' }}
                  </button>
                  <a routerLink="/events" class="btn btn-secondary">İptal</a>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class EventCreateComponent implements OnInit {
  eventForm: FormGroup;
  categories: Category[] = [];
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private eventService: EventService,
    private categoryService: CategoryService,
    private router: Router
  ) {
    this.eventForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      date: ['', Validators.required],
      location: ['', Validators.required],
      categoryId: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadCategories();
  }

  private loadCategories(): void {
    this.categoryService.getCategories().subscribe({
      next: (categories) => this.categories = categories,
      error: (error) => console.error('Error loading categories:', error)
    });
  }

  onSubmit(): void {
    if (this.eventForm.valid) {
      this.isLoading = true;
      this.eventService.createEvent(this.eventForm.value).subscribe({
        next: (event) => {
          this.router.navigate(['/events', event.id]);
        },
        error: (error) => {
          console.error('Error creating event:', error);
          this.isLoading = false;
        }
      });
    }
  }
} 