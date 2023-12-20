import sanitizeHtml from 'sanitize-html';

export type SanitizationRules = sanitizeHtml.IOptions;

export const richText: SanitizationRules = {
    allowedTags: ['p', 'br', 'b', 'i', 'em', 'strong', 'ul', 'ol', 'li', 'a'],
};

export const escapeAllTags: SanitizationRules = {
    allowedTags: [],
    disallowedTagsMode: 'recursiveEscape',
};

export const cleanHtml = (dirty: string, options: SanitizationRules) => sanitizeHtml(dirty, options);
