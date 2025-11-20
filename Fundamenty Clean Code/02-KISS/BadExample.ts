// Problem: Przekombinowana walidacja formularza z zagnieżdżonymi if-ami i dziwnymi flagami

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
    let isValid = true;
    let errors: string[] = [];
    let emailValid = false;
    let passwordValid = false;
    let ageValid = false;
    let termsValid = false;
    let validationStage = 0;

    // Zagnieżdżona walidacja z dziwnymi flagami i pętlami
    if (formData.email && formData.email.length > 0) {
      validationStage = 1;
      let atSignFound = false;
      let dotFound = false;
      let atSignPosition = -1;

      for (let i = 0; i < formData.email.length; i++) {
        if (formData.email[i] === "@") {
          atSignFound = true;
          atSignPosition = i;
        }
        if (formData.email[i] === "." && atSignFound && i > atSignPosition) {
          dotFound = true;
        }
      }

      if (atSignFound && dotFound && atSignPosition > 0) {
        if (formData.email.length - atSignPosition > 3) {
          emailValid = true;
          validationStage = 2;
        } else {
          errors.push("Email domain is too short");
          isValid = false;
        }
      } else {
        errors.push("Invalid email format");
        isValid = false;
      }
    } else {
      errors.push("Email is required");
      isValid = false;
    }

    if (validationStage >= 1) {
      if (formData.password) {
        if (formData.password.length >= 8) {
          let hasUpper = false;
          let hasLower = false;
          let hasNumber = false;

          for (let char of formData.password) {
            if (char >= "A" && char <= "Z") hasUpper = true;
            if (char >= "a" && char <= "z") hasLower = true;
            if (char >= "0" && char <= "9") hasNumber = true;
          }

          if (hasUpper && hasLower && hasNumber) {
            passwordValid = true;
            validationStage = 3;
          } else {
            if (!hasUpper) errors.push("Password must contain uppercase letter");
            if (!hasLower) errors.push("Password must contain lowercase letter");
            if (!hasNumber) errors.push("Password must contain number");
            isValid = false;
          }
        } else {
          errors.push("Password must be at least 8 characters");
          isValid = false;
        }
      } else {
        errors.push("Password is required");
        isValid = false;
      }
    }

    if (validationStage >= 2) {
      if (formData.age !== null && formData.age !== undefined) {
        if (typeof formData.age === "number") {
          if (formData.age >= 18) {
            if (formData.age <= 120) {
              ageValid = true;
              validationStage = 4;
            } else {
              errors.push("Age must be less than 120");
              isValid = false;
            }
          } else {
            errors.push("You must be at least 18 years old");
            isValid = false;
          }
        } else {
          errors.push("Age must be a number");
          isValid = false;
        }
      } else {
        errors.push("Age is required");
        isValid = false;
      }
    }

    if (validationStage >= 3) {
      if (formData.acceptTerms !== null && formData.acceptTerms !== undefined) {
        if (formData.acceptTerms === true) {
          termsValid = true;
        } else {
          errors.push("You must accept terms and conditions");
          isValid = false;
        }
      } else {
        errors.push("Terms acceptance is required");
        isValid = false;
      }
    }

    return { isValid: emailValid && passwordValid && ageValid && termsValid, errors };
  }
}
