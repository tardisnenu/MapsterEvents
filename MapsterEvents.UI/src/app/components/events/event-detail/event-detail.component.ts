import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { EventService } from '../../../services/event.service';
import { AuthService } from '../../../services/auth.service';
import { EventDetail } from '../../../models/event.model';

/**
 * Premium Event Detail Component
 * Created by Hamza Canturk - https://hamzacanturk.com/
 */
@Component({
  selector: 'app-event-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './event-detail.component.html',
  styleUrls: ['./event-detail.component.css']
})
export class EventDetailComponent implements OnInit, OnDestroy {
  event: EventDetail | null = null;
  isLoading = false;
  isRegistering = false;
  isRegistered = false;
  errorMessage = '';
  successMessage = '';
  
  private destroy$ = new Subject<void>();

  constructor(
    private eventService: EventService,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadEventDetail();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Load event details
   */
  private loadEventDetail(): void {
    const eventId = this.route.snapshot.params['id'];
    if (!eventId) {
      this.router.navigate(['/events']);
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.eventService.getEventById(parseInt(eventId))
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (event) => {
          this.event = event;
          this.isLoading = false;
          this.checkRegistrationStatus();
        },
        error: (error) => {
          console.error('Error loading event:', error);
          this.errorMessage = 'Etkinlik yüklenirken bir hata oluştu.';
          this.isLoading = false;
        }
      });
  }

  /**
   * Check if user is registered for this event
   */
  private checkRegistrationStatus(): void {
    if (!this.event || !this.isAuthenticated) {
      return;
    }

    this.eventService.getRegistrationStatus(this.event.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (isRegistered) => {
          this.isRegistered = isRegistered;
        },
        error: (error) => {
          console.error('Error checking registration status:', error);
        }
      });
  }

  /**
   * Register for event
   */
  registerForEvent(): void {
    if (!this.event || !this.isAuthenticated) {
      this.router.navigate(['/login'], { 
        queryParams: { returnUrl: this.router.url } 
      });
      return;
    }

    this.isRegistering = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.eventService.registerToEvent(this.event.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (message) => {
          this.isRegistering = false;
          this.isRegistered = true;
          this.successMessage = message || 'Etkinliğe başarıyla kayıt oldunuz!';
          // Refresh event details to update participant count
          this.loadEventDetail();
        },
        error: (error) => {
          this.isRegistering = false;
          this.errorMessage = error.error?.message || 'Kayıt olurken bir hata oluştu.';
          console.error('Error registering for event:', error);
        }
      });
  }

  /**
   * Unregister from event
   */
  unregisterFromEvent(): void {
    if (!this.event || !this.isAuthenticated) {
      return;
    }

    this.isRegistering = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.eventService.unregisterFromEvent(this.event.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (message) => {
          this.isRegistering = false;
          this.isRegistered = false;
          this.successMessage = message || 'Etkinlik kaydınız iptal edildi.';
          // Refresh event details to update participant count
          this.loadEventDetail();
        },
        error: (error) => {
          this.isRegistering = false;
          this.errorMessage = error.error?.message || 'Kayıt iptal edilirken bir hata oluştu.';
          console.error('Error unregistering from event:', error);
        }
      });
  }

  /**
   * Check if user is authenticated
   */
  get isAuthenticated(): boolean {
    return this.authService.isLoggedIn();
  }

  /**
   * Check if event is upcoming
   */
  get isUpcoming(): boolean {
    return this.event ? new Date(this.event.date) > new Date() : false;
  }

  /**
   * Check if user is the organizer
   */
  get isOrganizer(): boolean {
    if (!this.event || !this.isAuthenticated) {
      return false;
    }
    
    const currentUser = this.authService.currentUserValue;
    return currentUser?.email === this.event.organizerEmail;
  }

  /**
   * Format date for display
   */
  formatDate(date: string | Date): string {
    return new Date(date).toLocaleDateString('tr-TR', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  /**
   * Format time for display
   */
  formatTime(date: string | Date): string {
    return new Date(date).toLocaleTimeString('tr-TR', {
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  /**
   * Navigate to edit event
   */
  editEvent(): void {
    if (this.event) {
      this.router.navigate(['/events', this.event.id, 'edit']);
    }
  }

  /**
   * Share event
   */
  shareEvent(): void {
    if (navigator.share && this.event) {
      navigator.share({
        title: this.event.title,
        text: this.event.description,
        url: window.location.href
      }).catch(err => console.error('Error sharing:', err));
    } else {
      // Fallback: copy URL to clipboard
      navigator.clipboard.writeText(window.location.href).then(() => {
        this.successMessage = 'Etkinlik bağlantısı kopyalandı!';
        setTimeout(() => this.successMessage = '', 3000);
      });
    }
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