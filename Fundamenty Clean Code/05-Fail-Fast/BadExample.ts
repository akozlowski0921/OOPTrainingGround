// Problem: Wielopoziomowe zagnieżdżenia if - "Arrow Code"

interface User {
  id: number;
  email: string;
  isVerified: boolean;
  subscriptionType: "free" | "premium" | "enterprise";
}

interface Document {
  id: number;
  ownerId: number;
  isPublic: boolean;
  size: number;
}

interface AccessResult {
  canAccess: boolean;
  reason?: string;
}

export class DocumentAccessService {
  checkAccess(user: User | null, document: Document | null): AccessResult {
    // Zagnieżdżony kod trudny do śledzenia
    if (user) {
      if (user.isVerified) {
        if (document) {
          if (document.isPublic) {
            // Dostęp do publicznego dokumentu
            return { canAccess: true };
          } else {
            // Dokument prywatny
            if (document.ownerId === user.id) {
              // Właściciel ma dostęp
              return { canAccess: true };
            } else {
              // Nie właściciel - sprawdź subskrypcję
              if (user.subscriptionType === "premium" || user.subscriptionType === "enterprise") {
                // Premium może oglądać cudze prywatne dokumenty jeśli są małe
                if (document.size < 10000000) {
                  return { canAccess: true };
                } else {
                  // Tylko enterprise może oglądać duże pliki
                  if (user.subscriptionType === "enterprise") {
                    return { canAccess: true };
                  } else {
                    return {
                      canAccess: false,
                      reason: "Document too large for premium subscription",
                    };
                  }
                }
              } else {
                return {
                  canAccess: false,
                  reason: "Private document access requires premium subscription",
                };
              }
            }
          }
        } else {
          return { canAccess: false, reason: "Document not found" };
        }
      } else {
        return { canAccess: false, reason: "Email not verified" };
      }
    } else {
      return { canAccess: false, reason: "User not authenticated" };
    }
  }
}
