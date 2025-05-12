import i18n from 'i18next';
import {initReactI18next} from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';

i18n
    // detect user language
    // learn more: https://github.com/i18next/i18next-browser-languageDetector
    .use(LanguageDetector)
    // pass the i18n instance to react-i18next.
    .use(initReactI18next)
    // init i18next
    // for all options read: https://www.i18next.com/overview/configuration-options
    .init({
        debug: true,
        fallbackLng: 'en',
        interpolation: {
            escapeValue: false, // not needed for React as it escapes by default
        },
        resources: {
            en: {
                translation: {
                    login: "Log in to load attributes",
                    info: "Attributes can be loaded into your Yivi app. To load these attributes, it is necessary that you log in using the button below and if prompted give Yivi access to your profile data. We use this data only during attribute loading.",
                    backendError: "Something went wrong. Please try again.",
                }
            },
            nl: {
                translation: {
                    login: "Log in om attributen te laden",
                    info: "Attributen kunnen eenvoudig in de Yivi app geladen worden. Hiervoor dient u in te loggen via de knop hieronder en eventueel toestemming te geven. We gebruiken deze gegevens alleen tijdens het laden van de attributen in de app.",
                    backendError: "Er is iets misgegaan. Probeer het opnieuw.",
                }
            }
        }
    });

export default i18n;