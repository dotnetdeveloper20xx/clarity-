export interface PaginatedList<T> {
  items: T[];
  pageNumber: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface ApiError {
  type: string;
  title: string;
  status: number;
  correlationId: string;
  errors?: Record<string, string[]>;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  email: string;
  fullName: string;
  roles: string[];
}

export interface ClientDto {
  id: string;
  referenceNumber: string;
  name: string;
  clientType: number;
  status: number;
  email?: string;
  phone?: string;
  addressLine1?: string;
  city?: string;
  postCode?: string;
  country?: string;
  companyNumber?: string;
  notes?: string;
  createdAt: string;
}

export interface MatterDto {
  id: string;
  referenceNumber: string;
  clientId: string;
  clientName: string;
  title: string;
  description?: string;
  matterType: number;
  status: number;
  feeArrangement: number;
  estimatedValue?: number;
  openedDate: string;
  closedDate?: string;
  leadConsultantId: string;
  leadConsultantName: string;
  createdAt: string;
}

export interface TimeEntryDto {
  id: string;
  matterId: string;
  matterReference: string;
  matterTitle: string;
  userId: string;
  userName: string;
  date: string;
  durationMinutes: number;
  description: string;
  isBillable: boolean;
  rateAmount?: number;
  status: number;
  createdAt: string;
}

export interface InvoiceDto {
  id: string;
  invoiceNumber: string;
  clientId: string;
  clientName: string;
  matterId: string;
  matterReference: string;
  status: number;
  issueDate?: string;
  dueDate?: string;
  subTotal: number;
  taxAmount: number;
  totalAmount: number;
  paidAmount: number;
  outstandingAmount: number;
}
