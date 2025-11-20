// Rozwiązanie: Early return - spłaszczone warunki, łatwe do śledzenia

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
  private static readonly LARGE_FILE_SIZE = 10_000_000; // 10MB

  checkAccess(user: User | null, document: Document | null): AccessResult {
    // Fail fast - sprawdzamy warunki brzegowe najpierw
    if (!user) {
      return { canAccess: false, reason: "User not authenticated" };
    }

    if (!user.isVerified) {
      return { canAccess: false, reason: "Email not verified" };
    }

    if (!document) {
      return { canAccess: false, reason: "Document not found" };
    }

    // Dokument publiczny - każdy może zobaczyć
    if (document.isPublic) {
      return { canAccess: true };
    }

    // Właściciel zawsze ma dostęp
    if (document.ownerId === user.id) {
      return { canAccess: true };
    }

    // Dokument prywatny - wymaga subskrypcji
    if (user.subscriptionType === "free") {
      return {
        canAccess: false,
        reason: "Private document access requires premium subscription",
      };
    }

    // Enterprise ma dostęp do wszystkich dokumentów
    if (user.subscriptionType === "enterprise") {
      return { canAccess: true };
    }

    // Premium - tylko małe pliki
    if (document.size >= DocumentAccessService.LARGE_FILE_SIZE) {
      return {
        canAccess: false,
        reason: "Document too large for premium subscription",
      };
    }

    return { canAccess: true };
  }
}
