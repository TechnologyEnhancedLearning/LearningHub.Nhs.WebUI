# NHS Design System View Components

## Introduction
A set of View Components designed to allow .Net developers easy access to [NHS Design System Components](https://service-manual.nhs.uk/design-system/components).
Each View Component provides reliable implementation of the NHS Design System component markup so that instead of:
```
  <div class="nhsuk-form-group">
  <fieldset class="nhsuk-fieldset" aria-describedby="example-hint" role="group">
    <legend class="nhsuk-fieldset__legend nhsuk-label--l">
      <h1 class="nhsuk-fieldset__heading">
        What is your date of birth?
      </h1>
    </legend>
    <div class="nhsuk-hint" id="example-hint">
      For example, 15 3 1984
    </div>

    <div class="nhsuk-date-input" id="example">
      <div class="nhsuk-date-input__item">
        <div class="nhsuk-form-group">
          <label class="nhsuk-label nhsuk-date-input__label" for="example-day">
            Day
          </label>
          <input class="nhsuk-input nhsuk-date-input__input nhsuk-input--width-2" asp-for="day" id="example-day" name="example-day" type="text" pattern="[0-9]*" inputmode="numeric">
        </div>
      </div>
      <div class="nhsuk-date-input__item">
        <div class="nhsuk-form-group">
          <label class="nhsuk-label nhsuk-date-input__label" for="example-month">
            Month
          </label>
          <input class="nhsuk-input nhsuk-date-input__input nhsuk-input--width-2" asp-for="month" id="example-month" name="example-month" type="text" pattern="[0-9]*" inputmode="numeric">
        </div>
      </div>
      <div class="nhsuk-date-input__item">
        <div class="nhsuk-form-group">
          <label class="nhsuk-label nhsuk-date-input__label" for="example-year">
            Year
          </label>
          <input class="nhsuk-input nhsuk-date-input__input nhsuk-input--width-4" asp-for="year" id="example-year" name="example-year" type="text" pattern="[0-9]*" inputmode="numeric">
        </div>
      </div>
    </div>
  </fieldset>

</div>
```
In your Razor markup, you can use:
```
<vc:date-input id="date-of-birth"
                     label="What is your date of birth?"
                     day-id="Day"
                     month-id="Month"
                     year-id="Year"
                     css-class="nhsuk-u-margin-bottom-4"
                     hint-text-lines="For example, 15 3 1984" />
```
## Included Components

### Standard inputs
- Checkboxes
- NumericInput
- Radios
- SelectList
- SingleCheckbox
- TextArea
- TextInput

### Additional inputs
- FileInput
- DateInput
- DateRangeInput

### Validation
- ErrorSummary

### Navigation Components
- ActionLink
- BackLink
- CancelLink
