import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { EventService } from '../../../services/event.service';
import { CategoryService } from '../../../services/category.service';
import { EventListItem } from '../../../models/event.model';
import { Category } from '../../../models/category.model';

/**
 * Premium Event List Component
 * Created by Hamza Canturk - https://hamzacanturk.com/
 */
@Component({
  selector: 'app-event-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './event-list.component.html',
  styleUrls: ['./event-list.component.css']
})
export class EventListComponent implements OnInit, OnDestroy {
  events: EventListItem[] = [];
  categories: Category[] = [];
  filteredEvents: EventListItem[] = [];
  isLoading = false;
  searchForm: FormGroup;
  
  // Filter options
  selectedCategory: number | null = null;
  selectedStatus: string = 'all';
  currentPage = 1;
  itemsPerPage = 9;
  totalItems = 0;
  
  private destroy$ = new Subject<void>();

  constructor(
    private eventService: EventService,
    private categoryService: CategoryService,
    private router: Router,
    private route: ActivatedRoute,
    private fb: FormBuilder
  ) {
    this.searchForm = this.fb.group({
      searchTerm: [''],
      categoryId: [''],
      status: ['all']
    });
  }

  ngOnInit(): void {
    this.loadInitialData();
    this.setupSearchForm();
    this.handleRouteParams();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Load initial data (events and categories)
   */
  private loadInitialData(): void {
    this.isLoading = true;

    // Load events
    this.eventService.getAllEvents()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (events) => {
          this.events = events;
          this.applyFilters();
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading events:', error);
          this.isLoading = false;
        }
      });

    // Load categories
    this.categoryService.getCategories()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (categories) => {
          this.categories = categories;
        },
        error: (error) => {
          console.error('Error loading categories:', error);
        }
      });
  }

  /**
   * Setup search form with debouncing
   */
  private setupSearchForm(): void {
    this.searchForm.get('searchTerm')?.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        takeUntil(this.destroy$)
      )
      .subscribe(() => {
        this.applyFilters();
      });

    this.searchForm.get('categoryId')?.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe((categoryId) => {
        this.selectedCategory = categoryId ? parseInt(categoryId) : null;
        this.applyFilters();
      });

    this.searchForm.get('status')?.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe((status) => {
        this.selectedStatus = status;
        this.applyFilters();
      });
  }

  /**
   * Handle route parameters (for category filtering)
   */
  private handleRouteParams(): void {
    this.route.params
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        if (params['categoryId']) {
          this.selectedCategory = parseInt(params['categoryId']);
          this.searchForm.patchValue({ categoryId: params['categoryId'] });
        }
      });
  }

  /**
   * Apply filters to event list
   */
  applyFilters(): void {
    let filtered = [...this.events];
    
    // Search filter
    const searchTerm = this.searchForm.get('searchTerm')?.value?.toLowerCase();
    if (searchTerm) {
      filtered = filtered.filter(event => 
        event.title.toLowerCase().includes(searchTerm) ||
        event.shortDescription.toLowerCase().includes(searchTerm) ||
        event.location.toLowerCase().includes(searchTerm)
      );
    }

    // Category filter
    if (this.selectedCategory) {
      const selectedCategoryName = this.categories.find(c => c.id === this.selectedCategory)?.name;
      if (selectedCategoryName) {
        filtered = filtered.filter(event => event.categoryName === selectedCategoryName);
      }
    }

    // Status filter
    if (this.selectedStatus === 'upcoming') {
      filtered = filtered.filter(event => new Date(event.date) > new Date());
    } else if (this.selectedStatus === 'past') {
      filtered = filtered.filter(event => new Date(event.date) < new Date());
    }

    this.filteredEvents = filtered;
    this.totalItems = filtered.length;
    this.currentPage = 1; // Reset to first page
  }

  /**
   * Get events for current page
   */
  get pagedEvents(): EventListItem[] {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    return this.filteredEvents.slice(startIndex, startIndex + this.itemsPerPage);
  }

  /**
   * Get total pages
   */
  get totalPages(): number {
    return Math.ceil(this.totalItems / this.itemsPerPage);
  }

  /**
   * Get page numbers for pagination
   */
  get pageNumbers(): number[] {
    const pages = [];
    const maxPagesToShow = 5;
    let startPage = Math.max(1, this.currentPage - Math.floor(maxPagesToShow / 2));
    let endPage = Math.min(this.totalPages, startPage + maxPagesToShow - 1);

    if (endPage - startPage + 1 < maxPagesToShow) {
      startPage = Math.max(1, endPage - maxPagesToShow + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    return pages;
  }

  /**
   * Navigate to page
   */
  navigateToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  /**
   * Navigate to event detail
   */
  viewEventDetail(eventId: number): void {
    this.router.navigate(['/events', eventId]);
  }

  /**
   * Clear all filters
   */
  clearFilters(): void {
    this.searchForm.reset({
      searchTerm: '',
      categoryId: '',
      status: 'all'
    });
    this.selectedCategory = null;
    this.selectedStatus = 'all';
    this.applyFilters();
  }

  /**
   * Format date for display
   */
  formatDate(date: string | Date): string {
    return new Date(date).toLocaleDateString('tr-TR', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  /**
   * Check if event is upcoming
   */
  isUpcoming(date: string | Date): boolean {
    return new Date(date) > new Date();
  }

  /**
   * Handle image error
   */
  onImageError(event: Event, eventTitle: string): void {
    const target = event.target as HTMLImageElement;
    if (target) {
      target.src = this.getDefaultImageForEvent(eventTitle);
    }
  }

  /**
   * Get default image for event based on title
   */
  getDefaultImageForEvent(title: string): string {
    switch (title) {
      case '.NET 8 ve Modern Web Geliştirme':
        return '/images/NET 8 ve Modern Web.png';
      case 'Angular ve TypeScript Masterclass':
        return '/images/Angular ve TypeScript.png';
      case 'Yazılım Geliştirmede SOLID Prensipleri':
        return '/images/Yazılım Geliştirmede SOLID.png';
      case 'Startup Dünyasına Giriş':
        return '/images/Startup Dünyasına.png';
      case 'Açık Kaynak Kodlu Yazılım Geliştirme':
        return '/images/Açık Kaynak Kodlu.png';
      case 'Proje Yönetimi ve Agile Metodolojiler':
        return '/images/Proje Yönetimi ve Agile.png';
      default:
        return '/images/NET 8 ve Modern Web.png';
    }
  }
} 