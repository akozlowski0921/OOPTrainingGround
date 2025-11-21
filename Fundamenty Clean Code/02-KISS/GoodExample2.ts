// Rozwiązanie: Prosta implementacja używająca wbudowanych metod JavaScript

interface User {
  id: number;
  name: string;
  age: number;
  isActive: boolean;
  registrationDate: Date;
}

type SortField = "name" | "age";
type SortOrder = "asc" | "desc";

export class UserFilterService {
  filterAndSortUsers(
    users: User[],
    minAge?: number,
    maxAge?: number,
    activeOnly?: boolean,
    sortBy?: SortField,
    sortOrder: SortOrder = "asc"
  ): User[] {
    // Filtrowanie - czytelne, sekwencyjne
    let result = users.filter((user) => {
      if (minAge !== undefined && user.age < minAge) return false;
      if (maxAge !== undefined && user.age > maxAge) return false;
      if (activeOnly && !user.isActive) return false;
      return true;
    });

    // Sortowanie - używamy wbudowanej metody sort
    if (sortBy) {
      result.sort((a, b) => {
        const aValue = a[sortBy];
        const bValue = b[sortBy];
        
        const comparison = aValue > bValue ? 1 : aValue < bValue ? -1 : 0;
        return sortOrder === "desc" ? -comparison : comparison;
      });
    }

    return result;
  }
}
