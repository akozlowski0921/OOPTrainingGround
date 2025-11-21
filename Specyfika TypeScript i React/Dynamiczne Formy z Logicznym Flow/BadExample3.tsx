import React, { useState } from 'react';

// ❌ BAD: Multi-step wizard without proper state management

export function ManualWizardForm() {
  const [currentStep, setCurrentStep] = useState(1);
  const [step1Data, setStep1Data] = useState({ name: '', email: '' });
  const [step2Data, setStep2Data] = useState({ address: '', city: '' });
  const [step3Data, setStep3Data] = useState({ cardNumber: '', cvv: '' });
  const [errors, setErrors] = useState<Record<string, string>>({});

  // Problem: Validation logic dla każdego kroku osobno
  const validateStep1 = () => {
    const newErrors: Record<string, string> = {};
    if (!step1Data.name) newErrors.name = 'Required';
    if (!step1Data.email) newErrors.email = 'Required';
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const validateStep2 = () => {
    const newErrors: Record<string, string> = {};
    if (!step2Data.address) newErrors.address = 'Required';
    if (!step2Data.city) newErrors.city = 'Required';
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleNext = () => {
    if (currentStep === 1 && validateStep1()) {
      setCurrentStep(2);
      setErrors({});
    } else if (currentStep === 2 && validateStep2()) {
      setCurrentStep(3);
      setErrors({});
    }
  };

  const handleSubmit = () => {
    // Problem: Łączenie danych z różnych kroków
    const finalData = { ...step1Data, ...step2Data, ...step3Data };
    console.log('Submitted:', finalData);
  };

  return (
    <div>
      <div>Step {currentStep} of 3</div>
      
      {currentStep === 1 && (
        <div>
          <input
            placeholder="Name"
            value={step1Data.name}
            onChange={(e) => setStep1Data({ ...step1Data, name: e.target.value })}
          />
          {errors.name && <span>{errors.name}</span>}
          
          <input
            placeholder="Email"
            value={step1Data.email}
            onChange={(e) => setStep1Data({ ...step1Data, email: e.target.value })}
          />
          {errors.email && <span>{errors.email}</span>}
        </div>
      )}

      {currentStep === 2 && (
        <div>
          <input
            placeholder="Address"
            value={step2Data.address}
            onChange={(e) => setStep2Data({ ...step2Data, address: e.target.value })}
          />
          {errors.address && <span>{errors.address}</span>}
          
          <input
            placeholder="City"
            value={step2Data.city}
            onChange={(e) => setStep2Data({ ...step2Data, city: e.target.value })}
          />
          {errors.city && <span>{errors.city}</span>}
        </div>
      )}

      {currentStep === 3 && (
        <div>
          <input
            placeholder="Card Number"
            value={step3Data.cardNumber}
            onChange={(e) => setStep3Data({ ...step3Data, cardNumber: e.target.value })}
          />
          <input
            placeholder="CVV"
            value={step3Data.cvv}
            onChange={(e) => setStep3Data({ ...step3Data, cvv: e.target.value })}
          />
        </div>
      )}

      <div>
        {currentStep > 1 && <button onClick={() => setCurrentStep(currentStep - 1)}>Back</button>}
        {currentStep < 3 && <button onClick={handleNext}>Next</button>}
        {currentStep === 3 && <button onClick={handleSubmit}>Submit</button>}
      </div>
    </div>
  );
}
