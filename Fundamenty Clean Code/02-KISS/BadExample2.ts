// Problem: Przekombinowana funkcja do filtrowania i sortowania listy użytkowników

interface User {
  id: number;
  name: string;
  age: number;
  isActive: boolean;
  registrationDate: Date;
}

export class UserFilterService {
  filterAndSortUsers(
    users: User[],
    minAge?: number,
    maxAge?: number,
    activeOnly?: boolean,
    sortBy?: string,
    sortOrder?: string
  ): User[] {
    let result: User[] = [];
    let tempArray: User[] = [];
    let filtered: boolean[] = [];
    let sortIndices: number[] = [];

    // Krok 1: Inicjalizacja tablicy filtrów
    for (let i = 0; i < users.length; i++) {
      filtered[i] = true;
    }

    // Krok 2: Filtrowanie po wieku (zagnieżdżona logika)
    if (minAge !== undefined || maxAge !== undefined) {
      for (let i = 0; i < users.length; i++) {
        if (minAge !== undefined) {
          if (users[i].age < minAge) {
            filtered[i] = false;
          }
        }
        if (maxAge !== undefined) {
          if (users[i].age > maxAge) {
            filtered[i] = false;
          }
        }
      }
    }

    // Krok 3: Filtrowanie po statusie aktywności
    if (activeOnly !== undefined && activeOnly === true) {
      for (let i = 0; i < users.length; i++) {
        if (filtered[i] === true) {
          if (users[i].isActive !== true) {
            filtered[i] = false;
          }
        }
      }
    }

    // Krok 4: Budowanie tymczasowej tablicy
    for (let i = 0; i < users.length; i++) {
      if (filtered[i] === true) {
        tempArray.push(users[i]);
        sortIndices.push(tempArray.length - 1);
      }
    }

    // Krok 5: Sortowanie (ręczna implementacja bubble sort)
    if (sortBy !== undefined && sortBy.length > 0) {
      let swapped = true;
      while (swapped) {
        swapped = false;
        for (let i = 0; i < tempArray.length - 1; i++) {
          let shouldSwap = false;

          if (sortBy === "name") {
            if (sortOrder === "desc") {
              if (tempArray[i].name < tempArray[i + 1].name) {
                shouldSwap = true;
              }
            } else {
              if (tempArray[i].name > tempArray[i + 1].name) {
                shouldSwap = true;
              }
            }
          } else if (sortBy === "age") {
            if (sortOrder === "desc") {
              if (tempArray[i].age < tempArray[i + 1].age) {
                shouldSwap = true;
              }
            } else {
              if (tempArray[i].age > tempArray[i + 1].age) {
                shouldSwap = true;
              }
            }
          }

          if (shouldSwap) {
            let temp = tempArray[i];
            tempArray[i] = tempArray[i + 1];
            tempArray[i + 1] = temp;
            swapped = true;
          }
        }
      }
    }

    // Krok 6: Kopiowanie wyniku
    for (let i = 0; i < tempArray.length; i++) {
      result.push(tempArray[i]);
    }

    return result;
  }
}
