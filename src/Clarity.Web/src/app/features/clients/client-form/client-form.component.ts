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

      <form (ngSubmit)="onSubmit()" class="card-section max-w-2xl">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div class="form-control md:col-span-2">
            <label class="label"><span class="label-text">Client Name *</span></label>
            <input type="text" [(ngModel)]="client.name" name="name" class="input input-bordered" required />
          </div>

          <div class="form-control">
            <label class="label"><span class="label-text">Type *</span></label>
            <select [(ngModel)]="client.clientType" name="clientType" class="select select-bordered">
              <option [ngValue]="0">Individual</option>
              <option [ngValue]="1">Organisation</option>
            </select>
          </div>

          <div class="form-control">
            <label class="label"><span class="label-text">Email</span></label>
            <input type="email" [(ngModel)]="client.email" name="email" class="input input-bordered" />
          </div>

          <div class="form-control">
            <label class="label"><span class="label-text">Phone</span></label>
            <input type="text" [(ngModel)]="client.phone" name="phone" class="input input-bordered" />
          </div>

          <div class="form-control">
            <label class="label"><span class="label-text">Address</span></label>
            <input type="text" [(ngModel)]="client.addressLine1" name="address" class="input input-bordered" />
          </div>

          <div class="form-control">
            <label class="label"><span class="label-text">City</span></label>
            <input type="text" [(ngModel)]="client.city" name="city" class="input input-bordered" />
          </div>

          <div class="form-control">
            <label class="label"><span class="label-text">Post Code</span></label>
            <input type="text" [(ngModel)]="client.postCode" name="postCode" class="input input-bordered" />
          </div>
        </div>

        <div class="flex justify-end gap-3 mt-6">
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
  client: any = { name: '', clientType: 1, email: '', phone: '', addressLine1: '', city: '', postCode: '' };
  saving = false;
  error: string | null = null;

  constructor(private api: ClientApiService, private router: Router) {}

  onSubmit(): void {
    this.saving = true;
    this.error = null;
    this.api.createClient(this.client).subscribe({
      next: () => this.router.navigate(['/clients']),
      error: (err) => {
        this.error = err?.error?.title ?? 'Failed to create client.';
        this.saving = false;
      }
    });
  }
}
