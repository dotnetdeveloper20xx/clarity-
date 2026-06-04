import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ClientApiService } from '../../../core/services/client-api.service';

@Component({
  selector: 'app-client-form',
  standalone: true,
  imports: [FormsModule, RouterLink],
  template: `
    <div class="page-container">
      <div class="mb-4">
        <a routerLink="/clients" class="text-sm text-primary hover:underline">← Back to Clients</a>
      </div>

      <h1 class="page-title mb-6">New Client</h1>

      @if (error) {
        <div class="alert alert-error mb-4">{{ error }}</div>
      }

      @if (success) {
        <div class="alert alert-success mb-4">Client created successfully. Redirecting...</div>
      }

      <form (ngSubmit)="onSubmit()" class="card-section max-w-3xl">
        <h2 class="section-title mb-4">Client Details</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div class="form-control md:col-span-2">
            <label class="label"><span class="label-text font-medium">Client Name *</span></label>
            <input type="text" [(ngModel)]="client.name" name="name" class="input input-bordered" placeholder="e.g., Harrington & Partners LLP" required />
          </div>

          <div class="form-control">
            <label class="label"><span class="label-text font-medium">Client Type *</span></label>
            <select [(ngModel)]="client.clientType" name="clientType" class="select select-bordered">
              <option [ngValue]="0">Individual</option>
              <option [ngValue]="1">Organisation</option>
            </select>
          </div>

          <div class="form-control">
            <label class="label"><span class="label-text font-medium">Email</span></label>
            <input type="email" [(ngModel)]="client.email" name="email" class="input input-bordered" placeholder="contact@example.com" />
          </div>

          <div class="form-control">
            <label class="label"><span class="label-text font-medium">Phone</span></label>
            <input type="text" [(ngModel)]="client.phone" name="phone" class="input input-bordered" placeholder="0207 123 4567" />
          </div>

          @if (client.clientType === 1) {
            <div class="form-control">
              <label class="label"><span class="label-text font-medium">Company Number</span></label>
              <input type="text" [(ngModel)]="client.companyNumber" name="companyNumber" class="input input-bordered" placeholder="12345678" />
            </div>
          }
        </div>

        <h2 class="section-title mb-4 mt-8">Address</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div class="form-control md:col-span-2">
            <label class="label"><span class="label-text font-medium">Address Line 1</span></label>
            <input type="text" [(ngModel)]="client.addressLine1" name="address" class="input input-bordered" placeholder="45 Chancery Lane" />
          </div>

          <div class="form-control">
            <label class="label"><span class="label-text font-medium">City</span></label>
            <input type="text" [(ngModel)]="client.city" name="city" class="input input-bordered" placeholder="London" />
          </div>

          <div class="form-control">
            <label class="label"><span class="label-text font-medium">Post Code</span></label>
            <input type="text" [(ngModel)]="client.postCode" name="postCode" class="input input-bordered" placeholder="WC2A 1JE" />
          </div>

          <div class="form-control">
            <label class="label"><span class="label-text font-medium">Country</span></label>
            <input type="text" [(ngModel)]="client.country" name="country" class="input input-bordered" value="United Kingdom" placeholder="United Kingdom" />
          </div>
        </div>

        <div class="form-control mt-6">
          <label class="label"><span class="label-text font-medium">Notes</span></label>
          <textarea [(ngModel)]="client.notes" name="notes" class="textarea textarea-bordered" rows="3" placeholder="Any additional notes about this client..."></textarea>
        </div>

        <div class="flex justify-end gap-3 mt-8 pt-4 border-t border-base-300">
          <a routerLink="/clients" class="btn btn-ghost">Cancel</a>
          <button type="submit" class="btn btn-primary" [disabled]="saving">
            @if (saving) { <span class="loading loading-spinner loading-sm"></span> }
            Create Client
          </button>
        </div>
      </form>
    </div>
  `
})
export class ClientFormComponent {
  client: any = { name: '', clientType: 1, email: '', phone: '', addressLine1: '', city: '', postCode: '', country: 'United Kingdom', companyNumber: '', notes: '' };
  saving = false;
  error: string | null = null;
  success = false;

  constructor(private api: ClientApiService, private router: Router) {}

  onSubmit(): void {
    if (!this.client.name.trim()) {
      this.error = 'Client name is required.';
      return;
    }
    this.saving = true;
    this.error = null;
    this.api.createClient(this.client).subscribe({
      next: () => {
        this.success = true;
        setTimeout(() => this.router.navigate(['/clients']), 1000);
      },
      error: (err) => {
        this.error = err?.error?.title ?? 'Failed to create client.';
        this.saving = false;
      }
    });
  }
}
