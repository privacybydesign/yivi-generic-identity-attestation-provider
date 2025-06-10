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
        load: "languageOnly",
        supportedLngs: ['en', 'nl'],
        resources: {
            en: {
                translation: {
                    loginButton: "Log in to load attributes",
                    loginInfo: "Attributes can be loaded into your Yivi app. To load these attributes, it is necessary that you log in using the button below and if prompted give Yivi access to your profile data. We use this data only during attribute loading.",
                    backendError: "Something went wrong. Please try again.",
                    loadAttributesInfo: "The attributes below can be added to your Yivi app:",
                    success: "Congratulations! Your attributes have been added to your Yivi app. They should now be visible there. You can now use these attributes on any website that accepts them."
                }
            },
            nl: {
                translation: {
                    loginButton: "Log in om attributen te laden",
                    loginInfo: "Attributen kunnen eenvoudig in de Yivi app geladen worden. Hiervoor dient u in te loggen via de knop hieronder en eventueel toestemming te geven. We gebruiken deze gegevens alleen tijdens het laden van de attributen in de app.",
                    backendError: "Er is iets misgegaan. Probeer het opnieuw.",
                    loadAttributesInfo: "De volgende attributen kunnen nu in uw Yivi app geladen worden:",
                    success: "Gefeliciteerd! Uw attributen zijn nu in uw IRMA app geladen. Daar zijn ze nu zichtbaar. U kunt deze attributen gebruiken op iedere website die ze accepteert."
                }
            }
        }
    });

export default i18n;