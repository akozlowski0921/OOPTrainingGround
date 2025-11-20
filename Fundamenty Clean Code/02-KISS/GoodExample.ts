// Rozwiązanie: Prosta walidacja z early return i czytelnymi warunkami

interface FormData {
  email: string;
  password: string;
  age: number;
  acceptTerms: boolean;
}

interface ValidationResult {
  isValid: boolean;
  errors: string[];
}

export class RegistrationFormValidator {
  validate(formData: FormData): ValidationResult {
    const errors: string[] = [];

    // Email validation - early return on error
    if (!formData.email) {
      errors.push("Email is required");
    } else if (!this.isValidEmail(formData.email)) {
      errors.push("Invalid email format");
    }

    // Password validation - early return on error
    if (!formData.password) {
      errors.push("Password is required");
    } else if (formData.password.length < 8) {
      errors.push("Password must be at least 8 characters");
    } else if (!this.hasRequiredPasswordCharacters(formData.password)) {
      errors.push("Password must contain uppercase, lowercase, and number");
    }

    // Age validation - early return on error
    if (formData.age === null || formData.age === undefined) {
      errors.push("Age is required");
    } else if (formData.age < 18) {
      errors.push("You must be at least 18 years old");
    } else if (formData.age > 120) {
      errors.push("Age must be less than 120");
    }

    // Terms validation - simple check
    if (!formData.acceptTerms) {
      errors.push("You must accept terms and conditions");
    }

    return {
      isValid: errors.length === 0,
      errors,
    };
  }

  private isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  private hasRequiredPasswordCharacters(password: string): boolean {
    const hasUpper = /[A-Z]/.test(password);
    const hasLower = /[a-z]/.test(password);
    const hasNumber = /[0-9]/.test(password);
    return hasUpper && hasLower && hasNumber;
  }
}
