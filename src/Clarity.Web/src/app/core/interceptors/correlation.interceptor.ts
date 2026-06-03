import { HttpInterceptorFn } from '@angular/common/http';

export const correlationInterceptor: HttpInterceptorFn = (req, next) => {
  const correlationId = generateId();
  req = req.clone({
    setHeaders: { 'X-Correlation-Id': correlationId }
  });
  return next(req);
};

function generateId(): string {
  return Math.random().toString(36).substring(2, 14);
}
