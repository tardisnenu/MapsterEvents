import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { takeUntil, finalize } from 'rxjs/operators';
import { EventService } from '../../services/event.service';
import { CategoryService } from '../../services/category.service';
import { AuthService } from '../../services/auth.service';
import { EventListItem } from '../../models/event.model';
import { Category } from '../../models/category.model';

/**
 * Premium Home Page Component
 * Created by Hamza Canturk - https://hamzacanturk.com/
 */
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  featuredEvents: EventListItem[] = [];
  categories: Category[] = [];
  isLoading = false;
  
  stats = {
    totalEvents: 0,
    totalCategories: 0,
    totalUsers: 0,
    totalRegistrations: 0
  };

  private destroy$ = new Subject<void>();

  constructor(
    private eventService: EventService,
    private categoryService: CategoryService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadHomeData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Load all home page data
   */
  private loadHomeData(): void {
    this.isLoading = true;

    // Load featured events (upcoming events)
    this.eventService.getUpcomingEvents()
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.isLoading = false)
      )
      .subscribe({
        next: (events) => {
          this.featuredEvents = events.slice(0, 6); // Show only first 6 events
          this.stats.totalEvents = events.length;
        },
        error: (error) => {
          console.error('Error loading featured events:', error);
        }
      });

    // Load categories
    this.categoryService.getCategories()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (categories) => {
          this.categories = categories;
          this.stats.totalCategories = categories.length;
        },
        error: (error) => {
          console.error('Error loading categories:', error);
        }
      });

    // Simulate additional stats (in real app, these would come from API)
    this.simulateStats();
  }

  /**
   * Simulate stats data (replace with real API calls)
   */
  private simulateStats(): void {
    // In a real application, these would be actual API calls
    setTimeout(() => {
      this.stats.totalUsers = Math.floor(Math.random() * 1000) + 500;
      this.stats.totalRegistrations = Math.floor(Math.random() * 5000) + 2000;
    }, 1000);
  }

  /**
   * Get icon for category based on name
   */
  getCategoryIcon(categoryName: string): string {
    const iconMap: { [key: string]: string } = {
      'teknoloji': '💻',
      'technology': '💻',
      'eğitim': '📚',
      'education': '📚',
      'spor': '⚽',
      'sports': '⚽',
      'sanat': '🎭',
      'art': '🎭',
      'kültür': '🎭',
      'culture': '🎭',
      'iş': '💼',
      'business': '💼',
      'kariyer': '💼',
      'career': '💼',
      'müzik': '🎵',
      'music': '🎵',
      'yemek': '🍽️',
      'food': '🍽️',
      'seyahat': '✈️',
      'travel': '✈️',
      'sağlık': '🏥',
      'health': '🏥'
    };

    const normalizedName = categoryName.toLowerCase();
    
    // Check for exact matches first
    if (iconMap[normalizedName]) {
      return iconMap[normalizedName];
    }

    // Check for partial matches
    for (const key in iconMap) {
      if (normalizedName.includes(key) || key.includes(normalizedName)) {
        return iconMap[key];
      }
    }

    // Default icon
    return '📅';
  }

  /**
   * Check if user is authenticated
   */
  get isAuthenticated(): boolean {
    return this.authService.isLoggedIn();
  }

  /**
   * Navigate to event creation (with auth check)
   */
  createEvent(): void {
    if (!this.isAuthenticated) {
      console.info('Etkinlik oluşturmak için giriş yapmanız gerekiyor.');
      this.router.navigate(['/login'], { 
        queryParams: { returnUrl: '/create-event' } 
      });
      return;
    }

    this.router.navigate(['/create-event']);
  }

  /**
   * Navigate to registration with smooth UX
   */
  navigateToRegister(): void {
    this.router.navigate(['/register']);
  }

  /**
   * Handle category navigation
   */
  navigateToCategory(categoryId: number): void {
    this.router.navigate(['/events/category', categoryId]);
  }

  /**
   * Handle event detail navigation
   */
  navigateToEvent(eventId: number): void {
    this.router.navigate(['/events', eventId]);
  }

  /**
   * Scroll to next section smoothly
   */
  scrollToNextSection(): void {
    const element = document.querySelector('.stats-section');
    if (element) {
      element.scrollIntoView({ 
        behavior: 'smooth',
        block: 'start'
      });
    }
  }
}