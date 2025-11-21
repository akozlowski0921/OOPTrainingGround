// Rozwiązanie: Guard clauses i early return

interface Reservation {
  id: number;
  userId: number;
  roomId: number;
  checkIn: Date;
  checkOut: Date;
  guestCount: number;
}

interface Room {
  id: number;
  capacity: number;
  isAvailable: boolean;
  pricePerNight: number;
  requiresDeposit: boolean;
}

interface UserProfile {
  id: number;
  hasVerifiedPayment: boolean;
  membershipLevel: "basic" | "silver" | "gold";
  reservationHistory: number;
}

interface BookingResult {
  success: boolean;
  error?: string;
  totalPrice?: number;
}

export class HotelBookingService {
  private static readonly MIN_NIGHTS = 1;
  private static readonly MAX_NIGHTS = 30;
  private static readonly DEPOSIT_WAIVER_HISTORY_THRESHOLD = 5;
  private static readonly MILLISECONDS_PER_DAY = 1000 * 60 * 60 * 24;

  bookRoom(
    reservation: Reservation | null,
    room: Room | null,
    user: UserProfile | null
  ): BookingResult {
    // Fail fast - walidacja podstawowa
    if (!reservation) {
      return { success: false, error: "Reservation data missing" };
    }

    if (!room) {
      return { success: false, error: "Room not found" };
    }

    if (!user) {
      return { success: false, error: "User profile not found" };
    }

    // Walidacja dostępności
    if (!room.isAvailable) {
      return { success: false, error: "Room not available" };
    }

    // Walidacja pojemności
    if (reservation.guestCount > room.capacity) {
      return { success: false, error: "Room capacity exceeded" };
    }

    // Walidacja dat
    if (reservation.checkOut <= reservation.checkIn) {
      return { success: false, error: "Check-out must be after check-in" };
    }

    const nights = this.calculateNights(
      reservation.checkIn,
      reservation.checkOut
    );

    if (nights < HotelBookingService.MIN_NIGHTS || nights > HotelBookingService.MAX_NIGHTS) {
      return {
        success: false,
        error: `Stay must be between ${HotelBookingService.MIN_NIGHTS} and ${HotelBookingService.MAX_NIGHTS} nights`,
      };
    }

    // Walidacja płatności
    if (!user.hasVerifiedPayment) {
      return { success: false, error: "Payment method not verified" };
    }

    // Walidacja depozytu
    if (room.requiresDeposit && !this.isDepositWaived(user)) {
      return { success: false, error: "Deposit required for new users" };
    }

    // Wszystko OK - oblicz cenę
    const totalPrice = room.pricePerNight * nights;
    return { success: true, totalPrice };
  }

  private calculateNights(checkIn: Date, checkOut: Date): number {
    return (
      (checkOut.getTime() - checkIn.getTime()) /
      HotelBookingService.MILLISECONDS_PER_DAY
    );
  }

  private isDepositWaived(user: UserProfile): boolean {
    // Premium membership zwalnia z depozytu
    if (
      user.membershipLevel === "gold" ||
      user.membershipLevel === "silver"
    ) {
      return true;
    }

    // Historia rezerwacji zwalnia z depozytu
    if (user.reservationHistory >= HotelBookingService.DEPOSIT_WAIVER_HISTORY_THRESHOLD) {
      return true;
    }

    return false;
  }
}
