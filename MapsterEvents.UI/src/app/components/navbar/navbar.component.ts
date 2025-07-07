import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models/user.model';

/**
 * Premium Navbar Component
 * Created by Hamza Canturk - https://hamzacanturk.com/
 */
@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit, OnDestroy {
  currentUser$: Observable<User | null>;
  isMenuOpen = false;
  isLoading = false;
  private destroy$ = new Subject<void>();

  constructor(
    private authService: AuthService,
    private router: Router
  ) {
    this.currentUser$ = this.authService.currentUser$;
  }

  ngOnInit(): void {
    // Subscribe to auth state changes for loading states
    this.authService.currentUser$
      .pipe(takeUntil(this.destroy$))
      .subscribe(user => {
        this.isLoading = false;
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Toggle mobile menu
   */
  toggleMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
  }

  /**
   * Handle user logout with elegant UX
   */
  logout(event: Event): void {
    event.preventDefault();
    
    this.isLoading = true;
    
    try {
      this.authService.logout();
      this.isLoading = false;
      this.isMenuOpen = false; // Close mobile menu
      this.router.navigate(['/']);
      
      // Show success message in console for now
      console.log('Logout successful');
    } catch (error) {
      this.isLoading = false;
      console.error('Logout error:', error);
    }
  }

  /**
   * Check if user is authenticated
   */
  get isAuthenticated(): boolean {
    return this.authService.isLoggedIn();
  }

  /**
   * Get current user info
   */
  get currentUser(): User | null {
    return this.authService.currentUserValue;
  }
}