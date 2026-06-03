import { Component } from '@angular/core';

@Component({
  selector: 'app-compliance-list',
  standalone: true,
  template: `
    <div class="page-container">
      <h1 class="page-title mb-6">Compliance</h1>

      <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
        <div class="card-section border-l-4 border-green-500">
          <div class="data-label">Passed</div>
          <div class="text-2xl font-bold text-green-600 mt-1">8</div>
        </div>
        <div class="card-section border-l-4 border-amber-500">
          <div class="data-label">Pending</div>
          <div class="text-2xl font-bold text-amber-600 mt-1">3</div>
        </div>
        <div class="card-section border-l-4 border-red-500">
          <div class="data-label">Failed</div>
          <div class="text-2xl font-bold text-red-600 mt-1">1</div>
        </div>
        <div class="card-section border-l-4 border-purple-500">
          <div class="data-label">Under Investigation</div>
          <div class="text-2xl font-bold text-purple-600 mt-1">1</div>
        </div>
      </div>

      <div class="card-section">
        <h2 class="section-title mb-4">Recent Compliance Checks</h2>
        <div class="overflow-x-auto">
          <table class="table table-sm">
            <thead>
              <tr class="text-navy-600">
                <th>Client</th>
                <th>Check Type</th>
                <th>Risk Level</th>
                <th>Status</th>
                <th>Performed By</th>
                <th>Date</th>
              </tr>
            </thead>
            <tbody>
              <tr class="hover:bg-base-200">
                <td>Client 3 Ltd</td>
                <td>AML</td>
                <td><span class="badge badge-sm badge-success">Low</span></td>
                <td><span class="badge badge-sm badge-success">Pass</span></td>
                <td class="text-navy-500">David Brown</td>
                <td class="text-xs text-navy-400">2 days ago</td>
              </tr>
              <tr class="hover:bg-base-200">
                <td>Client 7 Ltd</td>
                <td>KYC</td>
                <td><span class="badge badge-sm badge-warning">Medium</span></td>
                <td><span class="badge badge-sm badge-warning">Pending</span></td>
                <td class="text-navy-500">—</td>
                <td class="text-xs text-navy-400">Today</td>
              </tr>
              <tr class="hover:bg-base-200">
                <td>Client 9 Ltd</td>
                <td>Conflict of Interest</td>
                <td><span class="badge badge-sm badge-error">High</span></td>
                <td><span class="badge badge-sm badge-error">Failed</span></td>
                <td class="text-navy-500">David Brown</td>
                <td class="text-xs text-navy-400">1 week ago</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `
})
export class ComplianceListComponent {}
