// Components (default exports in v10)
import {
    initHeader,
    initSkipLinks, 
    initRadios, 
    initCheckboxes 
} from 'nhsuk-frontend';

// Initialize components once DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    // V10: Call the initialization functions directly.
    initHeader();
    initSkipLinks();
    initRadios();
    initCheckboxes();
});