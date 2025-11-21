// Problem: Zagnieżdżenia i warunki zamiast wczesnej walidacji

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
  bookRoom(
    reservation: Reservation | null,
    room: Room | null,
    user: UserProfile | null
  ): BookingResult {
    // Głęboko zagnieżdżony kod
    if (reservation) {
      if (room) {
        if (user) {
          if (room.isAvailable) {
            if (reservation.guestCount <= room.capacity) {
              if (reservation.checkOut > reservation.checkIn) {
                const nights =
                  (reservation.checkOut.getTime() -
                    reservation.checkIn.getTime()) /
                  (1000 * 60 * 60 * 24);

                if (nights >= 1 && nights <= 30) {
                  if (user.hasVerifiedPayment) {
                    if (room.requiresDeposit) {
                      if (
                        user.membershipLevel === "gold" ||
                        user.membershipLevel === "silver"
                      ) {
                        // Członkowie silver/gold nie muszą wpłacać depozytu
                        const price = room.pricePerNight * nights;
                        return { success: true, totalPrice: price };
                      } else {
                        // Basic musi wpłacić depozyt
                        if (user.reservationHistory >= 5) {
                          // Jeśli ma historię, depozyt nie jest wymagany
                          const price = room.pricePerNight * nights;
                          return { success: true, totalPrice: price };
                        } else {
                          return {
                            success: false,
                            error: "Deposit required for new users",
                          };
                        }
                      }
                    } else {
                      // Nie wymaga depozytu
                      const price = room.pricePerNight * nights;
                      return { success: true, totalPrice: price };
                    }
                  } else {
                    return {
                      success: false,
                      error: "Payment method not verified",
                    };
                  }
                } else {
                  return {
                    success: false,
                    error: "Stay must be between 1 and 30 nights",
                  };
                }
              } else {
                return {
                  success: false,
                  error: "Check-out must be after check-in",
                };
              }
            } else {
              return { success: false, error: "Room capacity exceeded" };
            }
          } else {
            return { success: false, error: "Room not available" };
          }
        } else {
          return { success: false, error: "User profile not found" };
        }
      } else {
        return { success: false, error: "Room not found" };
      }
    } else {
      return { success: false, error: "Reservation data missing" };
    }
  }
}
