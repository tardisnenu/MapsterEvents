import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CategoryService } from '../../../services/category.service';
import { Category } from '../../../models/category.model';

/**
 * Category List Component
 * Created by Hamza Canturk - https://hamzacanturk.com/
 */
@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="container">
      <div class="row">
        <div class="col-12">
          <h2 class="mb-4">Kategoriler</h2>
          <div class="row">
            <div class="col-md-6 col-lg-4 mb-4" *ngFor="let category of categories">
              <div class="card">
                <div class="card-body">
                  <h5 class="card-title">{{ category.name }}</h5>
                  <p class="card-text">{{ category.description }}</p>
                  <a [routerLink]="['/events']" [queryParams]="{category: category.id}" class="btn btn-primary">
                    Etkinlikleri Görüntüle
                  </a>
                </div>
              </div>
            </div>
          </div>
          <div *ngIf="categories.length === 0" class="text-center py-5">
            <p class="text-muted">Henüz kategori bulunmamaktadır.</p>
          </div>
        </div>
      </div>
    </div>
  `
})
export class CategoryListComponent implements OnInit {
  categories: Category[] = [];

  constructor(private categoryService: CategoryService) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  private loadCategories(): void {
    this.categoryService.getCategories().subscribe({
      next: (categories) => this.categories = categories,
      error: (error) => console.error('Error loading categories:', error)
    });
  }
} 