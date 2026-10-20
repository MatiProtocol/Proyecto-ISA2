export class BookingRequest {
  bookerEmail = '';
  bookingDate = '';
  publicKey = '';
  hasPrescriptionAll = true;
  details: BookingRequestDetail[] = [];

  constructor(
    bookerEmail: string,
    bookingDate: string,
    publicKey: string,
    hasPrescriptionAll: boolean,
    details: BookingRequestDetail[],
  ) {
    this.bookerEmail = bookerEmail;
    this.bookingDate = bookingDate;
    this.hasPrescriptionAll = hasPrescriptionAll;
    this.publicKey = publicKey;
    this.details = details;
  }
}

export class BookingRequestDetail {
  code = '';
  quantity = 1;
  pharmacyId = 1;
  prescriptionImage? = '';

  constructor(code: string, quantity: number, pharmacyId: number, prescriptionImage?: string) {
    this.code = code;
    this.quantity = quantity;
    this.pharmacyId = pharmacyId;
    this.prescriptionImage = prescriptionImage;
  }
}

export interface BookingResponse {
  id: number;
  bookerEmail: string;
  bookingDate: string;
  confirmationDate: string;
  closureDate: string;
  totalAmount: number;
  publicKey: string;
  status: string;
  details: BookingDetailModelResponse[];
}

export interface BookingDetailModelResponse {
  id: number;
  code: string;
  name: string;
  quantity: number;
  price: number;
  pharmacyId: number;
  pharmacyName: string;
  prescriptionImage?: string;
}

export interface BookingEmployeeRequest {
  pharmacyId: number;
  approverName: string;
  bookingId: number;
  cancel: boolean;
}

export interface BookingEmployeeResponse {
  success: boolean;
}
